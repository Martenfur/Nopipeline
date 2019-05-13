# NoPipeline
A Monogame Content Pipeline enhancer.

WachMonoGameContect - alpha varsion

To add WachMonoGameContect to a Monofox project:
1) Open Properties window in a Monofox project
2) Build Events tab
3) Add into Pre-build event command line:
   <path to the tool>\WatchMonoGameContent.exe  $(ProjectDir) >  $(ProjectDir)\WatchMonoGameContent.log
  
WatchMonoGameContent.log contains log of the tool.  
