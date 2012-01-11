using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Net;
using System.Web;

namespace HeadlessHtmlTesting {
    // Derived from Blog Post by Rohit Argawal: 
    // http://refactoringaspnet.blogspot.com/2010/04/using-htmlagilitypack-to-get-and-post.html
    // Uses the HTML Agility Pack to more efficiently test gets and posts in a web page without a full browser.
    public class HeadlessHtmlSession {
        private bool _isPost;
        public HtmlDocument HtmlDoc { get; set; }

        /// <summary>
        /// System.Net.CookieCollection. Provides a collection container for instances of Cookie class 
        /// </summary>
        public CookieCollection Cookies { get; set; }

        /// <summary>
        /// Provide a key-value-pair collection of form elements 
        /// </summary>
        public FormElementCollection FormElements { get; set; }

        /// <summary>
        /// Makes a HTTP GET request to the given URL
        /// </summary>
        public string Get(string url) {
            _isPost = false;
            CreateWebRequestObject().Load(url);
            return HtmlDoc.DocumentNode.InnerHtml;
        }

        /// <summary>
        /// Makes a HTTP POST request to the given URL
        /// </summary>
        public string Post(string url) {
            _isPost = true;
            CreateWebRequestObject().Load(url, "POST");
            return HtmlDoc.DocumentNode.InnerHtml;
        }

        /// <summary>
        /// Creates the HtmlWeb object and initializes all event handlers. 
        /// </summary>
        private HtmlWeb CreateWebRequestObject() {
            HtmlWeb web = new HtmlWeb();
            web.UseCookies = true;
            web.PreRequest = new HtmlWeb.PreRequestHandler(OnPreRequest);
            web.PostResponse = new HtmlWeb.PostResponseHandler(OnAfterResponse);
            web.PreHandleDocument = new HtmlWeb.PreHandleDocumentHandler(OnPreHandleDocument);
            return web;
        }

        /// <summary>
        /// Event handler for HtmlWeb.PreRequestHandler. Occurs before an HTTP request is executed.
        /// </summary>
        protected bool OnPreRequest(HttpWebRequest request) {
            AddCookiesTo(request);               // Add cookies that were saved from previous requests
            if (_isPost) AddPostDataTo(request); // We only need to add post data on a POST request
            return true;
        }

        /// <summary>
        /// Event handler for HtmlWeb.PostResponseHandler. Occurs after a HTTP response is received
        /// </summary>
        protected void OnAfterResponse(HttpWebRequest request, HttpWebResponse response) {
            SaveCookiesFrom(response); // Save cookies for subsequent requests
        }

        /// <summary>
        /// Event handler for HtmlWeb.PreHandleDocumentHandler. Occurs before a HTML document is handled
        /// </summary>
        protected void OnPreHandleDocument(HtmlDocument document) {
            SaveHtmlDocument(document);
        }

        /// <summary>
        /// Assembles the Post data and attaches to the request object
        /// </summary>
        private void AddPostDataTo(HttpWebRequest request) {
            string payload = FormElements.AssemblePostPayload();
            byte[] buff = Encoding.UTF8.GetBytes(payload.ToCharArray());
            request.ContentLength = buff.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            System.IO.Stream reqStream = request.GetRequestStream();
            reqStream.Write(buff, 0, buff.Length);
        }

        /// <summary>
        /// Add cookies to the request object
        /// </summary>
        private void AddCookiesTo(HttpWebRequest request) {
            if (Cookies != null && Cookies.Count > 0) {
                request.CookieContainer.Add(Cookies);
            }
        }

        /// <summary>
        /// Saves cookies from the response object to the local CookieCollection object
        /// </summary>
        private void SaveCookiesFrom(HttpWebResponse response) {
            if (response.Cookies.Count > 0) {
                if (Cookies == null) Cookies = new CookieCollection();
                Cookies.Add(response.Cookies);
            }
        }

        /// <summary>
        /// Saves the form elements collection by parsing the HTML document
        /// </summary>
        private void SaveHtmlDocument(HtmlDocument document) {
            HtmlDoc = document;
            FormElements = new FormElementCollection(HtmlDoc);
        }
    }

    /// <summary>
    /// Represents a combined list and collection of Form Elements.
    /// </summary>
    public class FormElementCollection : Dictionary<string, string> {
        /// <summary>
        /// Constructor. Parses the HtmlDocument to get all form input elements. 
        /// </summary>
        public FormElementCollection(HtmlDocument htmlDoc) {
            var inputs = htmlDoc.DocumentNode.Descendants("input");
            foreach (var element in inputs) {
                AddInputElement(element);
            }

            var menus = htmlDoc.DocumentNode.Descendants("select");
            foreach (var element in menus) {
                AddMenuElement(element);
            }

            var textareas = htmlDoc.DocumentNode.Descendants("textarea");
            foreach (var element in textareas) {
                AddTextareaElement(element);
            }
        }

        /// <summary>
        /// Assembles all form elements and values to POST. Also html encodes the values.  
        /// </summary>
        public string AssemblePostPayload() {
            StringBuilder sb = new StringBuilder();
            foreach (var element in this) {
                string value = HttpUtility.UrlEncode(element.Value);
                sb.Append("&" + element.Key + "=" + value);
            }
            return sb.ToString().Substring(1);
        }

        private void AddInputElement(HtmlNode element) {
            string name = element.GetAttributeValue("name", "");
            string value = element.GetAttributeValue("value", "");
            string type = element.GetAttributeValue("type", "");

            if (string.IsNullOrEmpty(name)) return;

            switch (type.ToLower()) {
                case "checkbox":
                case "radio":
                    if (!ContainsKey(name)) Add(name, "");
                    string isChecked = element.GetAttributeValue("checked", "unchecked");
                    if (!isChecked.Equals("unchecked")) this[name] = value;
                    break;
                default:
                    try {
                        Add(name, value);
                    } catch (ArgumentException) {
                        Console.WriteLine("An input element " + name + " already exists.");
                    }
                    break;
            }
        }

        private void AddMenuElement(HtmlNode element) {
            string name = element.GetAttributeValue("name", "");
            var options = element.Descendants("option");

            if (string.IsNullOrEmpty(name)) return;

            // choose the first option as default
            var firstOp = options.First();
            string defaultValue = firstOp.GetAttributeValue("value", firstOp.NextSibling.InnerText);
            try {
                Add(name, defaultValue);
            } catch (ArgumentException) {
                Console.WriteLine("An input element " + name + " already exists.");
            }

            // check if any option is selected
            foreach (var option in options) {
                string selected = option.GetAttributeValue("selected", "notSelected");
                if (!selected.Equals("notSelected")) {
                    string selectedValue = option.GetAttributeValue("value", option.NextSibling.InnerText);
                    this[name] = selectedValue;
                }
            }
        }

        private void AddTextareaElement(HtmlNode element) {
            string name = element.GetAttributeValue("name", "");
            if (string.IsNullOrEmpty(name)) return;
            try {
                Add(name, element.InnerText);
            }  catch (ArgumentException) {
                Console.WriteLine("An input element " + name + " already exists.");
            }
        }
    }
}
