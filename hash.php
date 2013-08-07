<?php
if (isset($_GET["file"]))
	echo md5_file($_GET["file"]);
?>