using System;
using System.Security.Cryptography;
using System.Text;

namespace курсач3сервер
{
    internal static class CryptoString
    {
        private const string Key = "<RSAKeyValue><Modulus>5XiFsc0IL0A52Drup1UDQXAdXkZhcaf/5e0eL9fzHzwBMxQ89kokBSxXYl0TJxfLCz1v96XmTIaf15Vd0mcABtc6TI5z1FnAhZO8ANgTdR41YvOIiaVjA+b6phOGglJmI9bOq7myh8YvFB6PHw+E+p1Igr1an5r3UQHGXb1eNyk=</Modulus><Exponent>AQAB</Exponent><P>+1rbGcM4Kr8MIROgHCxoJvjDv5hioh3Nmwm0o7HyQLZjVvnCHDGO1+dM8iyWy2KC+cjhh1fdgUxk84YYVCEplw==</P><Q>6bYiTvOs/jtHi+jq6nZFvozLIEGtvbom9aZKnJwgzUvfRqUv6YbvS5skcobT6jbGokKvUPmmMpaxL0bsgZQ9Pw==</Q><DP>Ax48a5RlZPpbvylMKi1O2XTqkLzmNFakT1EOgZ1agP0CPHj6tHjU6c6/wJ1W/YzqTQj160TmxYzaD79RG/IRXw==</DP><DQ>cvbDHa9EU7L5WNt0Y21WlYtQiEeGnaOqcoAgh7VNdW9zH582WFul7r0cSrIEIFxdjYfcEasclBoIgscSpBM1pQ==</DQ><InverseQ>wi0zlQlreHvdQOTsDDc+PfbTg0lZeC/WgDpHggQinrAK/op5VXT5e4GReVrE3zrTSbwh/xhNOhb+YXTyj2INKw==</InverseQ><D>EpQRbvXEpd6zUDhlPhL58oBbQFi+Zu9NHpZ2DTWUr4CyMCqBTQdvpyOR55rVhCK/A6fzur8pyCRWKKQlNnW0YLIieg/6Hft68GtYMPfGA7spMShuxc7FlXZNEPik5w1IcOGqZYSWx9Tt2Tbm/NES7f/FqWf7ttCcqK+IzJPk7Qk=</D></RSAKeyValue>";

        public static string Encrypt(this string str)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(Key);
            var cryptoData = rsa.Encrypt(Encoding.UTF8.GetBytes(str), false);
            return Convert.ToBase64String(cryptoData);  
        }

        public static string Decrypt(this string str)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(Key);
            var cryptoData = rsa.Decrypt(Convert.FromBase64String(str), false);
            return Encoding.UTF8.GetString(cryptoData);
        }
    }
}
