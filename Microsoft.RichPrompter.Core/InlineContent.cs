using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.RichPrompter
{
    public abstract class InlineContent
        : BaseViewModel
    {
        protected InlineContent(XElement inline)
        {
            this.inline = inline;
        }

        public static InlineContent InlineFromElement(XElement element)
        {
            if (element.Name == TextElement)
            {
                return new TextInline(element);
            }
            throw new NotSupportedException($"{element.Name} element is not a supported inline element");
        }


        protected readonly XElement inline;

        private static readonly XName TextElement = CuesCardDocument.PrompterNS + "Text";
    }

    public class TextInline
        : InlineContent
    {
        public TextInline(XElement inline) : base(inline) { }

        public string Text => inline.Value;
    }
}
