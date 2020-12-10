using System;
using System.Collections.Generic;
using System.Text;

namespace VideoIndexerApiClient.Entities
{
    public class SearchResult
    {
        public string id;
        public string name;
        public string description;
        public DateTime? created;
        public DateTime? lastModified;
        public DateTime? lastIndexed;
        public string userName;
        public int durationInSeconds;
        public List<SearchMatch> searchMatches;
    }
}
