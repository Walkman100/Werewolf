VBC = vbnc
VBCFLAGS = -nologo -quiet -utf8output -rootnamespace:werewolf -target:exe
VBFILES = werewolf.vb AssemblyInfo.vb

werewolf: $(VBFILES)
	$(VBC) $(VBCFLAGS) -out:werewolf $(VBFILES)

release: werewolf
	mv werewolf werewolf.exe

clean:
	$(RM) werewolf
	$(RM) werewolf.exe
	$(RM) -r bin
# in case you had been using MonoDevelop

all: werewolf
