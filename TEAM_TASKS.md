# 🚀 AI Proposal Evaluator - Team Delegation Plan

To efficiently split the workload among 7 team members with minimal merge conflicts, we can divide the project based on its architectural boundaries. Each member will take ownership of a specific domain.

---

## 🧑‍💻 Member 1: Frontend & UI (Blazor)
**Goal:** Build the user-facing web interface.
**Responsibilities:**
- Manage everything in the `Pages/` and `Components/` directories.
- Build the dashboard to show past evaluations and scores.
- Create the proposal upload form and handle loading states.
- Style the application using CSS/Bootstrap/Tailwind in `wwwroot/`.
- Ensure the app is responsive and accessible.

## 🧑‍💻 Member 2: Core Orchestration & Database (Data Layer)
**Goal:** Tie everything together and persist data.
**Responsibilities:**
- Manage `EvaluationOrchestrator.cs`, `Data/AppDbContext.cs`, and `Models/`.
- Handle Entity Framework Core migrations and SQLite database schema.
- Coordinate the pipeline (calling all other services in order when a proposal is uploaded).
- Ensure data models (like `ProposalEvaluation`) hold all necessary fields.

## 🧑‍💻 Member 3: Document Parsing & Ingestion
**Goal:** Extract clean text and data from uploaded files.
**Responsibilities:**
- Own `DocumentParserService.cs`.
- Improve the `PdfPig` integration to extract text more accurately (e.g., handling tables, multi-column layouts, and images).
- Add support for other document types (Word documents `.docx`, plain text `.txt`).
- Handle file storage/cleanup for temporary uploaded files.

## 🧑‍💻 Member 4: Machine Learning & Novelty Detection
**Goal:** Determine how unique a proposal is compared to past data.
**Responsibilities:**
- Own `MlEvaluationService.cs` and `NoveltyService.cs`.
- Manage the `ML.NET` (Microsoft.ML) integration and model training/prediction.
- Improve the TF-IDF vectorization and cosine similarity logic to find similar past proposals in `data/past_projects.csv`.
- Tune the ML model hyperparameters for better scoring accuracy.

## 🧑‍💻 Member 5: Financial & Narrative Analysis
**Goal:** Analyze the budget and the text structure of the proposal.
**Responsibilities:**
- Own `FinancialService.cs` and `NarrativeService.cs`.
- Extract and analyze the budget numbers from the proposal text.
- Evaluate the narrative (is it well-written? Does it hit required sections like "Methodology" or "Expected Impact"?).
- Integrate with any external LLM APIs (like OpenAI) if you decide to use AI for deeper text analysis.

## 🧑‍💻 Member 6: Interactive Reviewer Chat
**Goal:** Allow users to "chat" with the AI about the proposal.
**Responsibilities:**
- Own `ReviewerChatService.cs` and the `/api/ask` endpoint in `Program.cs`.
- Set up the logic to answer user questions based on the proposal's text context (RAG - Retrieval-Augmented Generation).
- Manage prompt engineering to ensure the AI reviewer acts as a strict, helpful evaluator.

## 🧑‍💻 Member 7: Automated Report Generation
**Goal:** Generate beautiful PDF summaries of the evaluation.
**Responsibilities:**
- Own `ReportService.cs`.
- Use `QuestPDF` to design and generate a detailed PDF report for each uploaded proposal.
- Include tables, charts (if possible), scores, and the final decision in the PDF.
- Ensure the generated PDFs are correctly saved to the `/reports` directory and are accessible to users to download.

---

> **Workflow Tip:** Have each member create their own Git branch (e.g., `feature/report-generation` or `feature/ui-dashboard`). Because each person is primarily working in different files (`.cs` services or `.razor` pages), you will rarely encounter merge conflicts!
