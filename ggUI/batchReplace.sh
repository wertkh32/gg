#!/bin/bash.exe
FILES=$(ls *.cs | xargs)
echo $FILES
for i in $FILES
do
  #sed "s/ String / string /" $i > $i.temp
  mv $i.temp $i
done
