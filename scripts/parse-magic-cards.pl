use strict;
use warnings;
use HTML::Entities;

my $input = 'C:\Users\Sho\AppData\Local\Cockatrice\cards_nosets.xml';
my $output = 'C:\Users\Sho\git\nadeko\scripts\card_questions.json';
print "Starting parsing...\n";

open INPUT, "<:utf8", $input or die "Cannot open file: $!\n";
my $str;
while (<INPUT>){
	$str .= $_;
}
close INPUT;
print "Finished Parsing.\n";

open OUTPUT, ">:utf8", $output or die "Cannot open file: $!\n";
print OUTPUT "[\n";
my $first = 1;
while($str =~ /<card>\s+<name>([^<]+)<\/name>[\s\w<="\/>]+<manacost>([^<]*)<\/manacost>[\s\w<="\/>]+<type>([^<]+)<\/type>([\s\w<="\/>]+)<text>([^<]*)<\/text>\s+<\/card>\s*/gmi)
{
	if($first)
	{
		$first = 0;
	}
	else
	{
		print OUTPUT ",\n";
	} 

	my $name = $1;
	my $cost = $2;
	my $type = $3;
	my $pt = $4;
	my $text = $5;

	if($type =~ /creature/i)
	{
		#print "PT: $pt\n";
		#<STDIN>;
		$pt =~ /<pt>([^<]+)<\/pt>/;
		$pt = $1;
		#print "PT: $pt\n";
		#<STDIN>;
	}

	$name = decode_entities($name);
	$name =~ s/"//g;
	$text = decode_entities($text);
	$text =~ s/"//g;
	$text =~ s/\{/</g;
	$text =~ s/\}/>/g;
	$text =~ s/\s+/ /g;

	if($text =~ s/$name/CARDNAME/gi)
	{
		if($name =~ /^([^,]+),.*$/)
		{
			my $miniName = $1;
			$text =~ s/$miniName/CARDNAME/gi;
		}
	}

	print "NAME:$name:\n";
	print "COST:$cost:\n";
	print "TYPE:$type:\n";

	if($type =~ /creature/i)
	{
		print "PT:$pt:\n";
		#<STDIN>;
	}
	

	print "TEXT:$text:\n";
	#<STDIN>;

	print OUTPUT " {\n";
	print OUTPUT "  \"Question\":\"\\n$cost\\n$type\\n";
	if($type =~ /creature/i)
	{
		print OUTPUT "$pt\\n";
	}
	print OUTPUT "$text\",\n";
	print OUTPUT "  \"Answer\":\"$name\"\n";
	print OUTPUT " }";
}
print OUTPUT "\n]";
close OUTPUT;

