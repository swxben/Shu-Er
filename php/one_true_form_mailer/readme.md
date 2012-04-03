# The one true form mailer

One form mailer to rule them all.


## Usage
You only need `onetrueformmailer.php`, the rest of the files are for testing the script. All of the configuration is done at the top of `onetrueformmailer.php`. Replace `success.html` and `failure.html` with your own, or configure them to different paths in the script.

Build up the HTML email, using `$_REQUEST` to pull out the form values. Remember to use `eh()` or `ep()` to print the values, otherwise you could expose yourself to email header injection attacks.

There is an example which dumps out all fields in the submitted request, which could be useful if you don't want to maintain the script with the form, or don't neccessarily know what fields are coming in. Be aware that this may break if there are attachments.