using System.Net.Mime;
using RestSharp;

namespace Apps.BitbucketDataCenter.Extensions;

public static class RestResponseExtensions
{
    public static string GetFilenameFromDispositionHeader(this RestResponse response, string fallbackFilename)
    {
        string finalFileName = Path.GetFileName(fallbackFilename);
        
        Func<HeaderParameter, bool> selectPredicate = 
            x => x.Name.Equals("Content-Disposition", StringComparison.OrdinalIgnoreCase);
        var dispositionHeader = response.ContentHeaders?.FirstOrDefault(selectPredicate) ?? 
                                response.Headers?.FirstOrDefault(selectPredicate);

        if (dispositionHeader?.Value == null) 
            return finalFileName;
        
        try
        {
            var disposition = new ContentDisposition(dispositionHeader.Value);
            if (!string.IsNullOrWhiteSpace(disposition.FileName))
                finalFileName = disposition.FileName;
        }
        catch
        {
            // ignored
        }

        return finalFileName;
    }
}