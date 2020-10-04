# Authors
#	Aleksander Kovač https://github.com/akovac35
#
# This bash script will increase the last digit of all csproj files within the current
# solution folder which contain version element.

while read -r line ; do
    echo "Processing $line"
	# Need to use intermediate file because of perl locks
	cp $line{,.bak}
	(perl -pe 's/^(\s*<Version>)((\d+\.)*)(\d+)(.*)$/$1.$2.($4+1).$5/e' < $line.bak) > $line
	rm $line.bak
	
done < <(grep -ril --include \*.csproj -E "<Version>.+</Version>" ./)