# The one true form mailer

One form mailer to rule them all.


## Usage
You only need `onetrueformmailer.php`, the rest of the files are for testing the script.


### Configuration

All of the configuration is done at the top of `onetrueformmailer.php`. Replace `success.html` and `failure.html` with your own, or configure them to different paths in the script.

Fields from the request could be used in the configuration, for example the 'from' address could be set to `$_REQUEST['email']`. **This is not recommended** as it increases the risk of header injection, although checks are done before sending the email (in `sendIt()`).


### Email template

Build up the HTML email, using `$_REQUEST` to pull out the form values. Remember to use `eh()` or `ep()` to print the values, otherwise you could expose yourself to email header injection attacks.

Some helper methods exist for printing fields within the email template:

- `eh($s)` - converts to HTML entities (to reduce injection attacks) then prints
- `ep($s)` - converts to HTML entities, then replaces new line characters with line breaks, then prints. Useful for printing `textarea` fields.
- `pr($o)` - pretty prints an object wrapped in `<pre>` tags. Useful for debugging (`<?php pr($_REQUEST); ?> dumps the entire request)


This example template just dumps out all fields in the submitted request, which could be useful if you don't want to maintain the script with the form, or don't neccessarily know what fields are coming in.

	<html>
		<body>
			<dl>
				<?php foreach ($_REQUEST as $k => $v) { ?>
					<dt><?php eh($k); ?></dt>
					<dd><?php ep($v); ?></dd>
				<?php } ?>
			</dl>
		</body>
	</html>



### Attachments

Attachments are added one by one by calling:

	$otfm->addAttachment('form_input_name');

If a file isn't selected, `addAttachment()` will silently skip it, so validation needs to be done client-side or manually.

To add all attachments, do the following (but be aware that this would allow an attacker to abuse the attachments):

	foreach ($_FILES as $k => $v) $otfm->addAttachment($k);