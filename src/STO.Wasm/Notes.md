# Breadcrumb
- [ ] PlayerAtGame has /pags in url which is not a valida url
- [ ] PLayers<player>Transaction doe snot display the breadcrumb properly because it uses guid for player bec

        <ol class="d-lg-none breadcrumb">
            @foreach (var segment in _uri.Segments)
            {
                var segmentDisplay = _textInfo.ToTitleCase(segment).TrimEnd('/');
                var uriToSegment = _uri.ToString()[.._uri.ToString().LastIndexOf(segment, StringComparison.Ordinal)];
                @if (segment == "/")
                {
                    <li class="breadcrumb-item "><a href="/"><i class="fa-solid fa-home"></i></a></li>
                }
                else
                {
                    @if (segment.EndsWith("/"))
                    {
                        <li class="breadcrumb-item"><a href="/@uriToSegment">@segmentDisplay</a></li>
                    }
                    else
                    {
                        @if (string.IsNullOrEmpty(_pageTitle))
                        {
                            <li class="breadcrumb-item active">@segmentDisplay</li>
                        }
                        else
                        {
                            <li class="breadcrumb-item active">@_pageTitle</li>
                        }
                    }
                }
            }
        </ol>