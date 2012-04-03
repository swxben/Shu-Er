<?php

	$postData = 
		'contactname=Lord+Sauron&'.
		'email=darklord@ciplit.com.au%0ABcc:frodo@ciplit.com.au'.
		'&message=Sorry+about+that+whole+ring+thing.+No+hard+feelings%3F';

	$url = 'http://'.$_SERVER['HTTP_HOST'].dirname($_SERVER['PHP_SELF']).'/onetrueformmailer.php';

	$result = do_post_request($url, $postData);

	echo($result);




	// http://wezfurlong.org/blog/2006/nov/http-post-from-php-without-curl/
	function do_post_request($url, $data, $optional_headers = null) {
		$params = array('http' => array(
			'method' => 'POST',
			'content' => $data
		));
		if ($optional_headers !== null) {
			$params['http']['header'] = $optional_headers;
		}
		$ctx = stream_context_create($params);
		$fp = @fopen($url, 'rb', false, $ctx);
		if (!$fp) {
			throw new Exception("Problem with $url, $php_errormsg");
		}
		$response = @stream_get_contents($fp);
		if ($response === false) {
			throw new Exception("Problem reading data from $url, $php_errormsg");
		}
		return $response;
	}

?>