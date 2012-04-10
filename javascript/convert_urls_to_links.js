// A function that converts URLs and email addresses in free text into HTML links. Also converts
// links to youtube videos to inline players.
// Based on http://snipplr.com/view/13533/convert-text-urls-into-links/
// The example uses jQuery but the function itself shouldn't have any dependencies.
// Usage:
//		<div id="content">And now for a link to the example.com website!</div>
//		...
//		var content = $('#content');
//		content.html(convertUrlsToLinks(content.html()));
//		// #content is now 'And now for a link to the <a rel="external" href="http://example.com">example.com</a> website!'

var convertUrlsToLinks = function(content) {
	// normal urls
	content = content.replace(
		/((https?\:\/\/|ftp\:\/\/)|(www\.))(\S+)(\w{2,4})(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?/gi,
		function(url) {
			// special case for youtube links
			if (url.indexOf('youtube.com') != -1) {
				var regex = new RegExp(/[\\?&]v=([^&#]*)/);
				var results = regex.exec(url);
				if (results == null) {
					return url;
				}
				var youtubeID = results[1];
				var width = 425;
				var height = 344;
				var youtubeCode = 
					'<object width="'+width+'" height="'+height+'"><param name="movie" '+
					'value="http://www.youtube.com/v/'+youtubeID+'"></param><param name="allowFullScreen" '+
					'value="true"></param><param name="allowscriptaccess" value="always"></param><embed src="'+
					'http://www.youtube.com/v/'+youtubeID+'" type="application/x-shockwave-flash" '+
					'allowscriptaccess="always" allowfullscreen="true" '+
					'width="'+width+'" height="'+height+'"></embed></object>';
				url = youtubeCode + '<br/>';
				return url;
			}								
		
			var urlText = url;
			if (url.match('^https?:\/\/')) {
				urlText = url.replace(/^https?:\/\//i, '');
			} else {
				url = 'http://'+url;
			}
			urlText = urlText.replace(/www./i, '');
			return '<a rel="external" href="'+url+'">'+urlText+'</a>';
		}
	);
	
	// emails
	content = content.replace(
		/\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b/gi,
		function(email) {
			return '<a href="mailto:'+email+'">'+email+'</a>';
		}
	);
	
	return content;
};