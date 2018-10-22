using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.RichPrompter
{
    public abstract class BlockContent
        : BaseViewModel
    {
        protected BlockContent(XElement block)
        {
            this.block = block;
            inlines = Property(nameof(Inlines), GetInlines);
        }

        public static BlockContent BlockFromElement(XElement element)
        {
            if (element.Name == ParagraphElement)
            {
                return new ParagraphBlock(element);
            }
            throw new NotSupportedException($"{element.Name} element is not a supported block element");
        }


        public IReadOnlyCollection<InlineContent> Inlines => inlines.Value;
        private IReadOnlyCollection<InlineContent> GetInlines() => new List<InlineContent>(from ie in block.Elements()
                                                                                           select InlineContent.InlineFromElement(ie)).AsReadOnly();
        private readonly ViewModelProperty<IReadOnlyCollection<InlineContent>> inlines;


        private readonly XElement block;

        private static readonly XName ParagraphElement = CuesCardDocument.PrompterNS + "Paragraph";
    }

    public class ParagraphBlock
        : BlockContent
    {
        public ParagraphBlock(XElement block) : base(block) { }
    }
}
