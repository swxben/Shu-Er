<?php
// The one true form mailer
// CC BY-SA 3.0
// See https://github.com/ciplit/Shu-Er

// Do not move this line:
ob_start();

// user configuration
	// from, to, subject, redirect
$otfm = new OneTrueFormMailer(array(
	'redirectSuccess' => 'success.html',
	'redirectFailure' => 'failure.html',
	'from' => 'helloworld@ciplit.com.au',
	'to' => 'helloworld@ciplit.com.au',
	'subject' => 'One True Form Mailer Test'
));

// get form values
$formValues = $otfm->getFormValues($_REQUEST);

// build message to mail, use ob_XXX() methods to generate HTML message
?>
<html>
	<body>
		<pre>
			<?php var_dump($formValues); ?>
		</pre>
	</body>
</html>
<?php



// ----- NOTHING BELOW THIS LINE NEEDS TO BE CHANGED -----

// mail it
$message = ob_get_contents();
ob_end_clean();
$otfm->mail($message);

// do redirection
$otfm->failure();



function e($s) { echo($s); }
function h($s) { return htmlentities($s); }
function eh($s) { e(h($s)); }

class OneTrueFormMailer {
	var $settings = null;

	function OneTrueFormMailer($settings) {
		$this->rsettings = $settings;
	}

	function mail($message) {
		eh($message);die();
	}

	function getFormValues($request) {
		$formValues = array();
		foreach ($request as $key => $val) {
			$formValues[$key] = $val;
		}
		return $formValues;
	}

	function success() { $this->redirectTo($this->settings['redirectSuccess']); }
	function failure() { $this->redirectTo($this->settings['redirectFailure']); }
	function redirectTo($url) { 
		header('Status: 302');
		header("Location: {$url}");
		die();
	}
}


?>