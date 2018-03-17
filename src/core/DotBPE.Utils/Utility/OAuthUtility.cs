using DotBPE.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;


namespace DotBPE.Utils.Utility {
    /// <summary>represents OAuth Token</summary>
    #pragma warning disable 612, 618

    /// <summary>represents OAuth Token</summary>
    [DebuggerDisplay("Key = {Key}, Secret = {Secret}")]
    [DataContract]
    public abstract class Token {
        [DataMember(Order = 1)]
        public string Key { get; private set; }
        [DataMember(Order = 2)]
        public string Secret { get; private set; }

        /// <summary>for serialize.</summary>
        [Obsolete("this is used for serialize")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Token() {

        }

        public Token(string key, string secret) {


            Key = key;
            Secret = secret;
        }
    }

    /// <summary>represents OAuth AccessToken</summary>
    [DataContract]
    public class AccessToken : Token {
        /// <summary>for serialize.</summary>
        [Obsolete("this is used for serialize")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AccessToken() {

        }

        public AccessToken(string key, string secret)
            : base(key, secret) { }
    }

    /// <summary>represents OAuth RequestToken</summary>
    [DataContract]
    public class RequestToken : Token {
        /// <summary>
        /// for serialize.
        /// </summary>
        [Obsolete("this is used for serialize")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public RequestToken() {

        }

        public RequestToken(string key, string secret)
            : base(key, secret) { }
    }

    /// <summary>OAuth Response</summary>
    public class TokenResponse<T> where T : Token {
        public T Token { get; private set; }
        public ILookup<string, string> ExtraData { get; private set; }

        public TokenResponse(T token, ILookup<string, string> extraData) {
            //Precondition.NotNull(token, "token");
            //Precondition.NotNull(extraData, "extraData");

            Token = token;
            ExtraData = extraData;
        }
    }

    #pragma warning restore 612, 618

    public class HttpClientHelper {
        public class KnownKeys {
            public const string HeaderAuthorization = "Authorization";
            public const string HeaderMediaType = "Content-Type";
            public const string AuthorizationBearer = "Bearer";
        }

        public class KnownValues {
            public const string MediaTypeJson = "application/json";
            public const string MediaTypeMultiFormData = "multipart/form-data";
        }
    }

    public static class OAuthUtility {
        /// <summary>Escape RFC3986 String</summary>
        public static string UrlEncode(this string stringToEscape) {
            return Uri.EscapeDataString(stringToEscape)
                .Replace("!", "%21")
                .Replace("*", "%2A")
                .Replace("'", "%27")
                .Replace("(", "%28")
                .Replace(")", "%29");
        }

        public static string UrlDecode(this string stringToUnescape) {
            return UrlDecodeForPost(stringToUnescape)
                .Replace("%21", "!")
                .Replace("%2A", "*")
                .Replace("%27", "'")
                .Replace("%28", "(")
                .Replace("%29", ")");
        }

        public static string UrlDecodeForPost(this string stringToUnescape) {
            stringToUnescape = stringToUnescape.Replace("+", " ");
            return Uri.UnescapeDataString(stringToUnescape);
        }

        public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(string query, bool post = false) {
            var queryParams = query.TrimStart('?').Split('&')
               .Where(x => x != "")
               .Select(x => {
                   var xs = x.Split('=');
                   if (post) {
                       return new KeyValuePair<string, string>(xs[0].UrlDecode(), xs[1].UrlDecodeForPost());
                   } else {
                       return new KeyValuePair<string, string>(xs[0].UrlDecode(), xs[1].UrlDecode());
                   }
               });

            return queryParams;
        }

        public static string Wrap(this string input, string wrapper) {
            return wrapper + input + wrapper;
        }

        public static string ToString<T>(this IEnumerable<T> source, string separator) {
            return string.Join(separator, source);
        }
        //--- end move

        public delegate byte[] HashFunction(byte[] key, byte[] buffer);

        private static readonly Random random = new Random();

        /// <summary>
        /// <para>hashKey -> buffer -> hashedBytes</para>
        /// <para>ex:</para>
        /// <para>ComputeHash = (key, buffer) => { using (var hmac = new HMACSHA1(key)) { return hmac.ComputeHash(buffer); } };</para>
        /// <para>ex(WinRT): </para>
        /// <para>ComputeHash = (key, buffer) =></para>
        /// <para>{</para>
        /// <para>&#160;&#160;&#160;&#160;var crypt = Windows.Security.Cryptography.Core.MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");</para>
        /// <para>&#160;&#160;&#160;&#160;var keyBuffer = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(key);</para>
        /// <para>&#160;&#160;&#160;&#160;var cryptKey = crypt.CreateKey(keyBuffer);</para>
        /// <para>&#160;</para>
        /// <para>&#160;&#160;&#160;&#160;var dataBuffer = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(buffer);</para>
        /// <para>&#160;&#160;&#160;&#160;var signBuffer = Windows.Security.Cryptography.Core.CryptographicEngine.Sign(cryptKey, dataBuffer);</para>
        /// <para>&#160;</para>
        /// <para>&#160;&#160;&#160;&#160;byte[] value;</para>
        /// <para>&#160;&#160;&#160;&#160;Windows.Security.Cryptography.CryptographicBuffer.CopyToByteArray(signBuffer, out value);</para>
        /// <para>&#160;&#160;&#160;&#160;return value;</para>
        /// <para>};</para>
        /// </summary>
        public static HashFunction ComputeHash { private get; set; }

        static string GenerateSignature(string consumerSecret, Uri uri, HttpMethod method, Token token, IEnumerable<KeyValuePair<string, string>> parameters) {
            if (ComputeHash == null) {
                ComputeHash = (key, buffer) => { using (var hmac = new HMACSHA1(key)) { return hmac.ComputeHash(buffer); } };
                //throw new InvalidOperationException("ComputeHash is null, must initialize before call OAuthUtility.HashFunction = /* your computeHash code */ at once.");
            }

            var hmacKeyBase = consumerSecret.UrlEncode() + "&" + ((token == null) ? "" : token.Secret).UrlEncode();

            // escaped => unescaped[]
            var queryParams = ParseQueryString(uri.GetComponents(UriComponents.Query | UriComponents.KeepDelimiter, UriFormat.UriEscaped));

            var stringParameter = parameters
                .Where(x => x.Key.ToLower() != "realm")
                .Concat(queryParams)
                .Select(p => new { Key = p.Key.UrlEncode(), Value = p.Value.UrlEncode() })
                .OrderBy(p => p.Key, StringComparer.Ordinal)
                .ThenBy(p => p.Value, StringComparer.Ordinal)
                .Select(p => p.Key + "=" + p.Value)
                .ToString("&");
            var signatureBase = method.ToString() +
                "&" + uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.Unescaped).UrlEncode() +
                "&" + stringParameter.UrlEncode();

            var hash = ComputeHash(Encoding.UTF8.GetBytes(hmacKeyBase), Encoding.UTF8.GetBytes(signatureBase));
            return Convert.ToBase64String(hash).UrlEncode();
        }

        public static IEnumerable<KeyValuePair<string, string>> BuildBasicParameters(string consumerKey, string consumerSecret, string url, HttpMethod method, Token token = null, IEnumerable<KeyValuePair<string, string>> optionalParameters = null) {
            var parameters = new List<KeyValuePair<string, string>>(capacity: 7)
            {
                new KeyValuePair<string,string>("oauth_consumer_key", consumerKey),
                new KeyValuePair<string,string>("oauth_nonce", random.Next().ToString() ),
                new KeyValuePair<string,string>("oauth_timestamp", DateTime.UtcNow.ToUnixTimeSeconds().ToString() ),
                new KeyValuePair<string,string>("oauth_signature_method", "HMAC-SHA1" ),
                new KeyValuePair<string,string>("oauth_version", "1.0" )
            };
            if (token != null)
                parameters.Add(new KeyValuePair<string, string>("oauth_token", token.Key));
            if (optionalParameters != null)
                parameters = parameters.Concat(optionalParameters).ToList();

            var signature = GenerateSignature(consumerSecret, new Uri(url), method, token, parameters);

            parameters.Add(new KeyValuePair<string, string>("oauth_signature", signature));

            return parameters;
        }
    }
}

