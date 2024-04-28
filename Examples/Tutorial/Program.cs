using Netcaster.Frames;

var app = FrameApplication.Create(args);

app.MapFrameGet("/", "initial-frame")
    .WithLocalImage(AspectRatio.Square, "images/hello-world.jpg");

app.Run();
