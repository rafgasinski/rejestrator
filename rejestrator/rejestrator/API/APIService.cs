namespace rejestrator.API
{
    using System;
    using System.IO;
    using System.Net;
    using Utils;

    static class APIService
    {
        private const string BASE_URL = "http://localhost:80/rejestrator/api/";

        public static string makeRequest(HTTPMethod method, string endPoint, string bodyJSON = null)
        {
            string responseValue = string.Empty;

            string uri = $"{BASE_URL}{endPoint}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = method.ToString();

            if (request.Method == HTTPMethod.POST.ToString() || 
                request.Method == HTTPMethod.PUT.ToString())
            {
                if(bodyJSON == null)
                    return Error.NO_CONTENT;

                request.ContentType = "application/x-www-form-urlencoded";
                using(StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(bodyJSON);
                    writer.Close();
                }
            }

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                            reader.Close();
                        }
                    }
                }
            }
            catch(WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse res = ex.Response as HttpWebResponse;
                    if (res != null)
                    {
                        if (res.StatusCode == HttpStatusCode.NotFound)
                            return Error.NOT_FOUND;
                        else if (res.StatusCode == HttpStatusCode.BadRequest)
                            return Error.BAD_REQUEST;
                    }
                    else
                    {
                        return Error.OTHER;
                    }
                }
                else
                {
                    return Error.OTHER;
                }

            }
            catch(Exception _)
            {
                return Error.OTHER;
            }
            finally
            {
                if(response != null)
                {
                    ((IDisposable)response).Dispose();
                }
            }

            return responseValue;
        }
    }

    public enum HTTPMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }
}
