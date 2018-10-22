using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.RichPrompter
{
    public class CueDetail
        : BaseViewModel
    {
        public CueDetail(XElement cue)
        {
            this.cue = cue;
            blocks = Property(nameof(Blocks), GetBlocks);
        }


        public string Header => cue.Element(HeaderElement).Value;

        public IReadOnlyCollection<BlockContent> Blocks => blocks.Value;
        private IReadOnlyCollection<BlockContent> GetBlocks() => new List<BlockContent>(from be in cue.Element(ContentElement).Elements()
                                                                                        select BlockContent.BlockFromElement(be)).AsReadOnly();
        private readonly ViewModelProperty<IReadOnlyCollection<BlockContent>> blocks;


        public override string ToString()
        {
            return $"CueDetail: [Header = {Header}, Blocks = [{string.Join(", ", Blocks.Select(b => $"Inlines = {b.Inlines.Count}"))}]]";
        }


        private readonly XElement cue;

        private static readonly XName HeaderElement = CuesCardDocument.PrompterNS + "Header";
        private static readonly XName ContentElement = CuesCardDocument.PrompterNS + "Content";
    }
}
