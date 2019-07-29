using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Tomato.Gateway.Swagger
{
    public class XmlCommentResolver
    {

        private const string MemberXPath = "/doc/members/member";
        private const string SummaryTag = "summary";


        private Dictionary<string,string> MEMBER_CACHE = new Dictionary<string, string>();

        public XmlCommentResolver(List<string> xmlPaths)
        {
            //XElement booksFromFile = XElement.Load(@"books.xml");\
            LoadXmlFiles(xmlPaths);
        }

        private void LoadXmlFiles(List<string> xmlPaths)
        {
            if (xmlPaths != null && xmlPaths.Any())
            {
                foreach (var xml in xmlPaths)
                {
                    LoadXmlFile(xml);
                }
            }
        }

        private void LoadXmlFile(string xmlPath)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(xmlPath);

            XmlNodeList members = doc.SelectNodes(MemberXPath);

            foreach ( XmlNode node in members)
            {
                string comment = node.SelectSingleNode(SummaryTag)?.InnerXml.Replace("\r\n", "").Trim();
                if (!string.IsNullOrEmpty(comment))
                {
                    MEMBER_CACHE.Add(node.Attributes["name"].Value,comment);
                }
            }
        }

        public string GetMemberInfoComment(MemberInfo memberInfo)
        {
            var key =  XmlCommentsMemberNameHelper.GetMemberNameForMember(memberInfo);
            return this.MEMBER_CACHE.ContainsKey(key) ? this.MEMBER_CACHE[key] : "";
        }

        public string GetTypeComment(Type type)
        {
            var key =  XmlCommentsMemberNameHelper.GetMemberNameForType(type);
            return this.MEMBER_CACHE.ContainsKey(key) ? this.MEMBER_CACHE[key] : "";
        }
        public string GetMethodComment(MethodInfo methodInfo)
        {
            var key =  XmlCommentsMemberNameHelper.GetMemberNameForMethod(methodInfo);
            return this.MEMBER_CACHE.ContainsKey(key) ? this.MEMBER_CACHE[key] : "";
        }

    }
}
