<?php

// The one true form mailer
// CC BY-SA 3.0
// See https://github.com/ciplit/Shu-Er

// user configuration
	// from, to, subject, redirect
$redirectSuccess = 'success.html';
$redirectFailure = 'failure.html';

// get form values

// build message to mail, use ob_XXX() methods to generate HTML message






// ----- NOTHING BELOW THIS LINE NEEDS TO BE CHANGED -----

var $otfm = new OneTrueFormMailer();

// mail it

// do redirection
$otfm->redirectTo($redirectSuccess);

// methods hidden down below all the action...
class OneTrueFormMailer {
	function redirectTo($url) { 
		header('Status: 302');
		header("Location: {$url}");
		die();
	}
}


?>