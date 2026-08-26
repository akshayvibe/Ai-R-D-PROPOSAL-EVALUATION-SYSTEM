using AIProposalEvaluator.Data;
using AIProposalEvaluator.Models;
using AIProposalEvaluator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddControllers(); // optional for pure API consumers

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")
        ?? "Data Source=proposals.db"));

builder.Services.AddScoped<IDocumentParserService, DocumentParserService>();
builder.Services.AddSingleton<INoveltyService, NoveltyService>();
builder.Services.AddScoped<IFinancialService, FinancialService>();
builder.Services.AddScoped<IMlEvaluationService, MlEvaluationService>();
builder.Services.AddScoped<INarrativeService, NarrativeService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReviewerChatService, ReviewerChatService>();
builder.Services.AddScoped<IEvaluationOrchestrator, EvaluationOrchestrator>();

var app = builder.Build();

// Ensure DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    // Load past projects for novelty
    var novelty = scope.ServiceProvider.GetRequiredService<INoveltyService>();
    var candidates = new[]
    {
        Path.Combine(app.Environment.ContentRootPath, "data", "past_projects.csv"),
        Path.Combine(app.Environment.ContentRootPath, "..", "..", "data", "past_projects.csv"),
        Path.Combine(Directory.GetCurrentDirectory(), "data", "past_projects.csv")
    };
    var csvPath = candidates.FirstOrDefault(File.Exists) ?? candidates[0];
    novelty.LoadPastProjects(csvPath);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Serve generated reports
var reportsPath = Path.Combine(app.Environment.ContentRootPath, "reports");
Directory.CreateDirectory(reportsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(reportsPath),
    RequestPath = "/reports"
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Serve generated reports
var reportsPath = Path.Combine(app.Environment.ContentRootPath, "reports");
Directory.CreateDirectory(reportsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(reportsPath),
    RequestPath = "/reports"
});

app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// ---------- Minimal API endpoints (also usable by external clients) ----------
app.MapPost("/api/submit", async (HttpRequest request, IEvaluationOrchestrator orchestrator) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest(new { error = "multipart/form-data expected" });

    var form = await request.ReadFormAsync();
    var file = form.Files.GetFile("file");
    if (file is null || file.Length == 0)
        return Results.BadRequest(new { error = "PDF file is required" });

    if (!double.TryParse(form["budget"], out var budget))
        return Results.BadRequest(new { error = "Valid budget is required" });

    var result = await orchestrator.EvaluateAsync(file, budget);

    if (!string.IsNullOrEmpty(result.Error))
        return Results.BadRequest(new { error = result.Error });

    return Results.Ok(result);
}).DisableAntiforgery();

app.MapPost("/api/ask", async (ChatRequest req, IReviewerChatService chat) =>
{
    var summary = $"Final Score: {req.FinalScore}, Decision: {req.Decision}";
    var answer = await chat.AskAsync(req.Question, req.ProposalText, summary);
    return Results.Ok(new { answer });
}).DisableAntiforgery();

app.MapGet("/api/history", async (IEvaluationOrchestrator orchestrator) =>
{
    var history = await orchestrator.GetHistoryAsync();
    return Results.Ok(history);
});

app.Run();
