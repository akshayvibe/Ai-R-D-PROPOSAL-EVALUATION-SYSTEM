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
