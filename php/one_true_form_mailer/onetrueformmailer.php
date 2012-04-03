<?php
// The one true form mailer
// CC BY-SA 3.0
// See https://github.com/ciplit/Shu-Er

// Configure the mailer. Avoid putting values from the form in this configuration.
$otfm = new OneTrueFormMailer(array(
	'success' => 'success.html',
	'failure' => 'failure.html',
	'from' => 'helloworld@ciplit.com.au',
	'to' => 'helloworld@ciplit.com.au',
	'subject' => 'One True Form Mailer Test'
));

// Get form values from the request (sanitised for header injection)
$formValues = $otfm->getFormValues($_REQUEST);
// remove this line if empty forms are OK
if (empty($formValues)) $otfm->failure();

// Build up your HTML email (use eh() etc to further sanitise form values for HTML):
?>
<html>
	<body>
		<p>Received a contact message:</p>
		<p>
			<strong>Name:</strong> <?php eh($formValues['contactname']); ?><br/>
			<strong>Email:</strong> <a href="mailto:<?php eh($formValues['email']); ?>"><?php eh($formValues['email']); ?></a><br/>
			<strong>Message:</strong>
		</p>
		<blockquote>
			<?php ep($formValues['message']); ?><br/>
		</blockquote>
	</body>
</html>
<?php



// ----- NOTHING BELOW THIS LINE NEEDS TO BE CHANGED -----



$otfm->sendIt();

function e($s) { echo($s); }
function h($s) { return htmlentities($s); }
function eh($s) { e(h($s)); }
function ep($s) { e(nl2br(h($s))); }

class OneTrueFormMailer {
	var $settings = null;

	function OneTrueFormMailer($settings) {
		$this->settings = $settings;
		ob_clean();
		ob_start();
	}

	function sendIt() {
		$message = ob_get_contents();
		ob_end_clean();

		extract($this->settings);
		if (!isset($to)) $to = $this->settings['to'];
		if (!isset($subject)) $subject = $this->settings['subject'];
		if (!isset($from)) $from = $to;
		
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

		$result = mail(
			$to,
			$subject,
			$content,
			$headers
			);
		if (!$result) die();// return $this->failure();
		return $this->success();
	}

	function getFormValues($request) {
		$formValues = array();
		foreach ($request as $key => $val) {
			$formValues[$key] = $val;
		}
		return $formValues;
	}

	function success() { $this->redirectTo($this->settings['success']); }
	function failure() { $this->redirectTo($this->settings['failure']); }
	function redirectTo($url) { 
		header('Status: 302');
		header("Location: {$url}");
		die();
	}
}


?>