using System;
using System.Web;

namespace FortyFingers.FilecuumCleaner.Library
{
    public class CookieUtil
    {

        // SetTripleDESEncryptedCookie - key & value only
        public static void SetTripleDESEncryptedCookie(string key, string value)
        {
            // Convert parts
            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                key = EncryptionUtils.EncryptTripleDES(key);
                value = EncryptionUtils.EncryptTripleDES(value);
            }
            SetCookie(key, value);
        }

        // SetTripleDESEncryptedCookie - overloaded method with expires parameter
        public static void SetTripleDESEncryptedCookie(string key, string value, DateTime expires)
        {
            // Convert parts
            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                key = EncryptionUtils.EncryptTripleDES(key);
                value = EncryptionUtils.EncryptTripleDES(value);
            }
            SetCookie(key, value, expires);
        }

        // SetEncryptedCookie - key & value only
        public static void SetEncryptedCookie(string key, string value)
        {
            // Convert parts
            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                key = EncryptionUtils.Encrypt(key);
                value = EncryptionUtils.Encrypt(value);
            }
            SetCookie(key, value);
        }

        // SetEncryptedCookie - overloaded method with expires parameter
        public static void SetEncryptedCookie(string key, string value, DateTime expires)
        {
            // Convert parts
            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                key = EncryptionUtils.Encrypt(key);
                value = EncryptionUtils.Encrypt(value);
            }
            SetCookie(key, value, expires);
        }

        // SetCookie - key & value only
        public static void SetCookie(string key, string value)
        {
            // Encode Part
            key = HttpContext.Current.Server.UrlEncode(key);
            value = HttpContext.Current.Server.UrlEncode(value);
            HttpCookie cookie;
            cookie = new HttpCookie(key, value);
            SetCookie(cookie);
        }

        // SetCookie - overloaded with expires parameter
        public static void SetCookie(string key, string value, DateTime expires)
        {
            // Encode Parts
            key = HttpContext.Current.Server.UrlEncode(key);
            value = HttpContext.Current.Server.UrlEncode(value);
            HttpCookie cookie;
            cookie = new HttpCookie(key, value);
            cookie.Expires = expires;
            SetCookie(cookie);
        }

        // SetCookie - HttpCookie only
        // final step to set the cookie to the response clause
        public static void SetCookie(HttpCookie cookie)
        {
            DeleteCookie(cookie.Name);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void DeleteEncryptedCookie(string key)
        {
            if (HttpContext.Current.Response.Cookies[EncryptionUtils.Encrypt(key)] != null)
            {
                HttpContext.Current.Response.Cookies.Remove(EncryptionUtils.Encrypt(key));
            }
        }

        public static void DeleteCookie(string key)
        {
            if (HttpContext.Current.Response.Cookies[key] != null)
            {
                HttpContext.Current.Response.Cookies.Remove(key);
            }
        }

        // GET COOKIE FUNCTIONS *****************************************************
        public static string GetTripleDESEncryptedCookieValue(string key)
        {
            // encrypt key only - encoding done in GetCookieValue
            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                key = EncryptionUtils.EncryptTripleDES(key);
            }
            // get value 
            string value;
            value = GetCookieValue(key);
            // decrypt value
            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                value = EncryptionUtils.DecryptTripleDES(value);
            }
            return value;
        }

        public static HttpCookie GetTripleDESEncryptedCookie(string key)
        {
            // encrypt key only - encoding done in GetCookieValue
            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                key = EncryptionUtils.EncryptTripleDES(key);
            }
            return GetCookie(key);
        }

        public static string GetEncryptedCookieValue(string key)
        {
            // encrypt key only - encoding done in GetCookieValue
            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                key = EncryptionUtils.Encrypt(key);
            }
            // get value 
            string value;
            value = GetCookieValue(key);
            // decrypt value
            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                value = EncryptionUtils.Decrypt(value);
            }
            return value;
        }

        public static HttpCookie GetEncryptedCookie(string key)
        {
            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                key = EncryptionUtils.Encrypt(key);
            }
            return GetCookie(key);
        }

        public static HttpCookie GetCookie(string key)
        {
            // encode key for retrieval
            key = HttpContext.Current.Server.UrlEncode(key);
            return HttpContext.Current.Request.Cookies.Get(key);
        }

        public static string GetCookieValue(string key)
        {
            try
            {
                // don't encode key for retrieval here
                // done in the GetCookie function
                // get value 
                string value;
                value = GetCookie(key).Value;
                // decode stored value
                value = HttpContext.Current.Server.UrlDecode(value);
                return value;
            }
            catch
            {
                return "";
            }
        }
    }
}