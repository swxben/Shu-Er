<?php
// The one true form mailer
// CIPL IT Services - ciplit.com.au
// CC BY-SA 3.0
// See https://github.com/ciplit/Shu-Er

// Configure the mailer.
// Avoid putting values from the form in this configuration.
$otfm = new OneTrueFormMailer(array(
	'success' => 'success.html',
	'failure' => 'failure.html',
	'from' => 'helloworld@ciplit.com.au',
	'to' => 'helloworld@ciplit.com.au',
	'subject' => 'One True Form Mailer Test'
));

// remove this line if empty forms are OK
if (empty($_REQUEST)) $otfm->failure();

// Add attachments (modify or remove as required):
$otfm->addAttachment('attachment1');
$otfm->addAttachment('attachment2');
$otfm->addAttachment('attachment3');

// Build up your HTML email:
?>
<html>
	<body>
		<p>Received a contact message:</p>
		<p>
			<strong>Name:</strong> <?php eh($_REQUEST['contactname']); ?><br/>
			<strong>Email:</strong> <a href="mailto:<?php eh($_REQUEST['email']); ?>"><?php eh($_REQUEST['email']); ?></a><br/>
			<strong>Message:</strong>
		</p>
		<blockquote>
			<?php ep($_REQUEST['message']); ?><br/>
		</blockquote>
	</body>
</html>
<?php





// -------------------------------------------------------
// ----- NOTHING BELOW THIS LINE NEEDS TO BE CHANGED -----
// -------------------------------------------------------

$otfm->sendIt();

function e($s) { echo($s); }
function h($s) { return htmlentities($s); }
function eh($s) { e(h($s)); }
function ep($s) { e(nl2br(h($s))); }
function pr($o) { e('<pre>'); print_r($o); e('</pre>'); }
function str_contains($source, $search) {
	if (!is_array($search)) return strpos($source, $search) !== FALSE;
	foreach ($search as $s) if (str_contains($source, $s)) return true;
	return false;
}

class OneTrueFormMailer {
	var $settings = null;
	var $attachments = array();

	function OneTrueFormMailer($settings) {
		$this->settings = $settings;
		ob_clean();
		ob_start();
	}

	function addAttachment($k) {
		if (!isset($_FILES[$k])) return;
		
		$a = $_FILES[$k];
		
		if (!isset($a['error'])) return;
		if ($a['error'] == 4) return;

		$this->attachments[$a['name']] = array(
			'type' => $a['type'],
			'data' =>$this->__readPostedFile($a['tmp_name'])
		);
	}

	function sendIt() {
		$message = ob_get_contents();
		ob_end_clean();

		extract($this->settings);
		if (!isset($to)) $to = $this->settings['to'];
		if (!isset($subject)) $subject = $this->settings['subject'];
		if (!isset($from)) $from = $to;

		$this->__checkForHeaderInjection(array($message, $to, $subject, $from));
		$this->__checkReferrer();
		
		$boundary = md5(uniqid(time()));
		
		$headers = 
			"From: {$from}".PHP_EOL.
			"Return-Path: {$from}".PHP_EOL.
			"Reply-To: {$from}".PHP_EOL.
			"MIME-Version: 1.0".PHP_EOL.
			"Content-Type: multipart/mixed; boundary=\"{$boundary}\"".PHP_EOL;

		$content =
			"--{$boundary}".PHP_EOL.
			"Content-type: text/html; charset=iso-8859-1".PHP_EOL.
			"Content-Transfer-Encoding: 7bit".PHP_EOL.
			PHP_EOL.
			$message.PHP_EOL.
			PHP_EOL;

		foreach ($this->attachments as $k => $v) {
			$attachment = chunk_split(base64_encode($v['data']));
			$content .=
				"--{$boundary}".PHP_EOL.
				"Content-type: application/octet-stream; name=\"{$k}\"".PHP_EOL.
				"Content-Transfer-Encoding: base64".PHP_EOL.
				"Content-Disposition: attachment; filename=\"{$k}\"".PHP_EOL.
				PHP_EOL.
				$attachment.PHP_EOL.
				PHP_EOL;				
		}

		$content .= "--{$boundary}--";

		$result = mail(
			$to,
			$subject,
			$content,
			$headers
			);
		if (!$result) return $this->failure();
		return $this->success();
	}

	function success() { $this->redirectTo($this->settings['success']); }
	function failure() { $this->redirectTo($this->settings['failure']); }
	function redirectTo($url) { 
		header('Status: 302');
		header("Location: {$url}");
		die();
	}

	function __checkForHeaderInjection($inputs) {
		$injectionPrej = 
			'/\b^to+(?=:)\b|^content-type:|^cc:|^bcc:|^from:|^subject:|^mime-version:|^content-transfer-encoding:/im';
		foreach ($inputs as $s) {
			if (preg_match($injectionPrej, $s)) $this->failure();
		}
	}

	function __checkReferrer() {
		if (empty($_SERVER['HTTP_REFERER'])) $this->failure();
		if (!str_contains($_SERVER['HTTP_REFERER'], $_SERVER['HTTP_HOST'])) $this->failure();
	}

	function __readPostedFile($f) {
		if (!is_uploaded_file($f)) throw new Exception('Invalid uploaded file location');		
		if (!file_exists($f)) throw new Exception('File does not exist');
		
		$handle = @fopen($f, 'rb');
		if ($handle === FALSE) throw new Exception('File could not be opened. Please try again.');
		
		$size = filesize($f);
		if ($size == 0) throw new Exception('File has zero length. Please try again.');
		
		$data = @fread($handle, $size);
		if ($data === FALSE) throw new Exception('File could not be read. Please try again.');
		
		fclose($handle);
		
		return $data;
	}
}


?>