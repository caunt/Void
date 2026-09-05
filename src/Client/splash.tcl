wm title . "Void Client Startup"
wm attributes . -fullscreen 1
. configure -bg black
label .l -text "Starting Minecraft...\n\n\nThis can take up to 1 minute." -font {Helvetica 30} -fg white -bg black
pack .l -expand 1 -fill both
