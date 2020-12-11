using System;
using System.Collections.Generic;
using System.Text;

namespace VideoIndexerApiClient.Entities
{
    public class SearchResults
    {
        public List<SearchResult> results;
        public PageInfo nextPage;
    }
}
