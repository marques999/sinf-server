using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FirstREST.LibPrimavera
{
    public class HashGenerator
    {
        public const string DEFAULT_ALPHABET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        public const string DEFAULT_SEPS = "cfhistuCFHISTU";

        private const int MIN_ALPHABET_LENGTH = 16;
        private const double SEP_DIV = 3.5;
        private const double GUARD_DIV = 12.0;

        private string alphabet;
        private string salt;
        private string seps;
        private string guards;
        private int minHashLength;

        private Regex guardsRegex;
        private Regex sepsRegex;

#if CORE
        private static Lazy<Regex> hexValidator = new Lazy<Regex>(() => new Regex("^[0-9a-fA-F]+$"));
        private static Lazy<Regex> hexSplitter = new Lazy<Regex>(() => new Regex(@"[\w\W]{1,12}"));
#else
        private static Lazy<Regex> hexValidator = new Lazy<Regex>(() => new Regex("^[0-9a-fA-F]+$", RegexOptions.Compiled));
        private static Lazy<Regex> hexSplitter = new Lazy<Regex>(() => new Regex(@"[\w\W]{1,12}", RegexOptions.Compiled));
#endif

        public HashGenerator()
            : this(string.Empty, 0, DEFAULT_ALPHABET, DEFAULT_SEPS)
        {
        }

        private HashGenerator(string salt = "", int minHashLength = 0, string alphabet = DEFAULT_ALPHABET, string seps = DEFAULT_SEPS)
        {
            if (string.IsNullOrWhiteSpace(alphabet))
            {
                throw new ArgumentNullException("alphabet");
            }

            this.salt = salt;
            this.seps = seps;
            this.minHashLength = minHashLength;
            this.alphabet = new string(alphabet.ToCharArray().Distinct().ToArray());

            if (this.alphabet.Length < 16)
            {
                throw new ArgumentException("alphabet must contain atleast 4 unique characters.", "alphabet");
            }

            SetupSeps();
            SetupGuards();
        }

        public virtual string Encode(params int[] numbers)
        {
            if (numbers.Any(n => n < 0))
            {
                return string.Empty;
            }

            return GenerateHashFrom(numbers.Select(n => (long)n).ToArray());
        }

        public virtual string Encode(IEnumerable<int> numbers)
        {
            return Encode(numbers.ToArray());
        }

        public virtual int[] Decode(string hash)
        {
            return GetNumbersFrom(hash).Select(n => (int)n).ToArray();
        }

        public virtual string EncodeHex(string hex)
        {
            if (!hexValidator.Value.IsMatch(hex))
            {
                return string.Empty;
            }

            var numbers = new List<long>();
            var matches = hexSplitter.Value.Matches(hex);

            foreach (Match match in matches)
            {
                numbers.Add(Convert.ToInt64(string.Concat("1", match.Value), 16));
            }

            return this.EncodeLong(numbers.ToArray());
        }

        public virtual string DecodeHex(string hash)
        {
            var ret = new StringBuilder();
            var numbers = this.DecodeLong(hash);

            foreach (var number in numbers)
            {
                ret.Append(string.Format("{0:X}", number).Substring(1));
            }

            return ret.ToString();
        }

        public long[] DecodeLong(string hash)
        {
            return this.GetNumbersFrom(hash);
        }

        public string EncodeLong(params long[] numbers)
        {
            if (numbers.Any(n => n < 0))
            {
                return string.Empty;
            }

            return GenerateHashFrom(numbers);
        }

        public string EncodeLong(IEnumerable<long> numbers)
        {
            return EncodeLong(numbers.ToArray());
        }

        private void SetupSeps()
        {
            seps = new string(seps.ToCharArray().Intersect(alphabet.ToCharArray()).ToArray());
            alphabet = new string(alphabet.ToCharArray().Except(seps.ToCharArray()).ToArray());
            seps = ConsistentShuffle(seps, salt);

            if (seps.Length == 0 || (alphabet.Length / seps.Length) > SEP_DIV)
            {
                var sepsLength = (int)Math.Ceiling(alphabet.Length / SEP_DIV);

                if (sepsLength == 1)
                {
                    sepsLength = 2;
                }

                if (sepsLength > seps.Length)
                {
                    var diff = sepsLength - seps.Length;
                    seps += alphabet.Substring(0, diff);
                    alphabet = alphabet.Substring(diff);
                }

                else
                {
                    seps = seps.Substring(0, sepsLength);
                }
            }

#if CORE
            sepsRegex = new Regex(string.Concat("[", seps, "]"));
#else
            sepsRegex = new Regex(string.Concat("[", seps, "]"), RegexOptions.Compiled);
#endif
            alphabet = ConsistentShuffle(alphabet, salt);
        }

        private void SetupGuards()
        {
            var guardCount = (int)Math.Ceiling(alphabet.Length / GUARD_DIV);

            if (alphabet.Length < 3)
            {
                guards = seps.Substring(0, guardCount);
                seps = seps.Substring(guardCount);
            }
            else
            {
                guards = alphabet.Substring(0, guardCount);
                alphabet = alphabet.Substring(guardCount);
            }

#if CORE
            guardsRegex = new Regex(string.Concat("[", guards, "]"));
#else
            guardsRegex = new Regex(string.Concat("[", guards, "]"), RegexOptions.Compiled);
#endif
        }

        private string GenerateHashFrom(long[] numbers)
        {
            if (numbers == null || numbers.Length == 0)
            {
                return string.Empty;
            }

            var ret = new StringBuilder();
            var alphabet = this.alphabet;
            long numbersHashInt = 0;

            for (var i = 0; i < numbers.Length; i++)
            {
                numbersHashInt += (int)(numbers[i] % (i + 100));
            }

            var lottery = alphabet[(int)(numbersHashInt % alphabet.Length)];

            ret.Append(lottery.ToString());

            for (var i = 0; i < numbers.Length; i++)
            {
                var number = numbers[i];
                var buffer = lottery + this.salt + alphabet;

                alphabet = ConsistentShuffle(alphabet, buffer.Substring(0, alphabet.Length));

                var last = Hash(number, alphabet);

                ret.Append(last);

                if (i + 1 < numbers.Length)
                {
                    number %= ((int)last[0] + i);
                    ret.Append(this.seps[((int)number % this.seps.Length)]);
                }
            }

            if (ret.Length < this.minHashLength)
            {
                var guardIndex = ((int)(numbersHashInt + (int)ret[0]) % this.guards.Length);
                var guard = this.guards[guardIndex];

                ret.Insert(0, guard);

                if (ret.Length < this.minHashLength)
                {
                    guardIndex = ((int)(numbersHashInt + (int)ret[2]) % this.guards.Length);
                    guard = this.guards[guardIndex];
                    ret.Append(guard);
                }
            }

            var halfLength = (int)(alphabet.Length / 2);

            while (ret.Length < this.minHashLength)
            {
                alphabet = ConsistentShuffle(alphabet, alphabet);
                ret.Insert(0, alphabet.Substring(halfLength));
                ret.Append(alphabet.Substring(0, halfLength));

                var excess = ret.Length - this.minHashLength;

                if (excess > 0)
                {
                    ret.Remove(0, excess / 2);
                    ret.Remove(this.minHashLength, ret.Length - this.minHashLength);
                }
            }

            return ret.ToString();
        }

        private string Hash(long input, string alphabet)
        {
            var hash = new StringBuilder();

            do
            {
                hash.Insert(0, alphabet[(int)(input % alphabet.Length)]);
                input = (input / alphabet.Length);
            } while (input > 0);

            return hash.ToString();
        }

        private long Unhash(string input, string alphabet)
        {
            long number = 0;

            for (var i = 0; i < input.Length; i++)
            {
                number += (long)(alphabet.IndexOf(input[i]) * Math.Pow(alphabet.Length, input.Length - i - 1));
            }

            return number;
        }

        private long[] GetNumbersFrom(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                return new long[0];
            }

            int i = 0;
            var ret = new List<long>();
            var alphabet = new string(this.alphabet.ToCharArray());
            var hashBreakdown = guardsRegex.Replace(hash, " ");
            var hashArray = hashBreakdown.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (hashArray.Length == 3 || hashArray.Length == 2)
            {
                i = 1;
            }

            hashBreakdown = hashArray[i];

            if (hashBreakdown[0] != default(char))
            {
                var lottery = hashBreakdown[0];

                hashBreakdown = hashBreakdown.Substring(1);
                hashBreakdown = sepsRegex.Replace(hashBreakdown, " ");
                hashArray = hashBreakdown.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                for (var j = 0; j < hashArray.Length; j++)
                {
                    var subHash = hashArray[j];
                    var buffer = lottery + this.salt + alphabet;

                    alphabet = ConsistentShuffle(alphabet, buffer.Substring(0, alphabet.Length));
                    ret.Add(Unhash(subHash, alphabet));
                }

                if (EncodeLong(ret.ToArray()) != hash)
                {
                    ret.Clear();
                }
            }

            return ret.ToArray();
        }

        private string ConsistentShuffle(string alphabet, string salt)
        {
            if (string.IsNullOrWhiteSpace(salt))
            {
                return alphabet;
            }

            int n;
            var letters = alphabet.ToCharArray();

            for (int i = letters.Length - 1, v = 0, p = 0; i > 0; i--, v++)
            {
                v %= salt.Length;
                p += (n = salt[v]);
                var j = (n + v + p) % i;
                var temp = letters[j];
                letters[j] = letters[i];
                letters[i] = temp;
            }

            return new string(letters);
        }
    }
}