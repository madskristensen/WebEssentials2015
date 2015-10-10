using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.Html.Core.Tree.Nodes;
using Microsoft.Html.Editor.SuggestedActions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace MadsKristensen.EditorExtensions.Html
{
    internal class IntegrityLightBulbAction : HtmlSuggestedActionBase
    {
        public IntegrityLightBulbAction(ITextView textView, ITextBuffer textBuffer, ElementNode element)
            : base(textView, textBuffer, element, "Calculate integrity")
        { }

        public override void Invoke(CancellationToken cancellationToken)
        {
            AttributeNode src = Element.GetAttribute("src") ?? Element.GetAttribute("href");
            AttributeNode integrity = Element.GetAttribute("integrity");
            string url = src.Value;

            if (url.StartsWith("//"))
            {
                url = "http:" + url;
            }

            string hash = CalculateHash(url);

            using (WebEssentialsPackage.UndoContext((DisplayText)))
            {
                using (ITextEdit edit = TextBuffer.CreateEdit())
                {
                    if (integrity != null)
                    {
                        Span span = new Span(integrity.ValueRangeUnquoted.Start, integrity.ValueRangeUnquoted.Length);
                        edit.Replace(span, hash);
                    }
                    else
                    {
                        edit.Insert(src.ValueRange.End, " integrity=\"" + hash + "\"");
                    }

                    edit.Apply();
                }
            }
        }

        private static string CalculateHash(string url)
        {
            StringBuilder sb = new StringBuilder();

            using (WebClient client = new WebClient())
            {
                byte[] bytes = client.DownloadData(new Uri(url));

                HashAlgorithm hasher = SHA256.Create();
                sb.AppendLine($"sha256-{Convert.ToBase64String(hasher.ComputeHash(bytes))}");

                hasher = SHA384.Create();
                sb.AppendLine($"sha384-{Convert.ToBase64String(hasher.ComputeHash(bytes))}");

                hasher = SHA512.Create();
                sb.AppendLine($"sha512-{Convert.ToBase64String(hasher.ComputeHash(bytes))}");
            }

            return sb.ToString().Trim();
        }
    }
}