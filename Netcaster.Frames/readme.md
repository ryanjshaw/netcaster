
    // TODO WithDynamicButtons(state => { })
    // return a list of buttons that will only be rendered this time

    // DOCS: Alternative pattern
    //app.MapGet("/", initialFrame);
    //app.MapPost("/themes", themesFrame);
    //app.MapPost("/themes/trippy", trippyThemeFrame);
    //app.MapPost("/themes/token", tokensThemeFrame);
    //app.MapPost("/themes/token/{token}", tokenStep1Frame);

    /*
     * State Management
     * 
     * Two options:
     *   URL segments => easy to use, but minimal type safety
     *   fc:frame:state => full type safety serialized
     * 
     */