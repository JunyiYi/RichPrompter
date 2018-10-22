using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;

namespace Microsoft.RichPrompter
{
    public class CuesCardDocument
        : BaseViewModel
    {
        public CuesCardDocument()
        {
            cuesDoc = Property<XDocument>(nameof(cuesDoc));
            title = Property(nameof(Title), GetTitle).DependOn(cuesDoc);
            cues = Property(nameof(Cues), GetCues).DependOn(cuesDoc);
            LoadCuesCardCommand = Command<string>(LoadCuesCard);
        }

        public string Title => title.Value;
        private string GetTitle() => cuesDoc.Value?.Root?.Element(TitleElement)?.Value;
        private readonly ViewModelProperty<string> title;

        public IReadOnlyList<CueDetail> Cues => cues.Value;
        private IReadOnlyList<CueDetail> GetCues() => new List<CueDetail>(from ce in cuesDoc.Value?.Descendants(CueElement) ?? Enumerable.Empty<XElement>()
                                                                          select new CueDetail(ce)).AsReadOnly();
        private readonly ViewModelProperty<IReadOnlyList<CueDetail>> cues;

        private readonly ViewModelProperty<XDocument> cuesDoc;


        public void LoadCuesCard(string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                cuesDoc.Value = XDocument.Load(uri);
            }
        }
        public ICommand LoadCuesCardCommand { get; }
        

        internal static readonly XNamespace PrompterNS = "http://schemas.microsoft.com/richprompter/2018";
        private static readonly XName TitleElement = PrompterNS + "Title";
        private static readonly XName CueElement = PrompterNS + "Cue";
    }
}
