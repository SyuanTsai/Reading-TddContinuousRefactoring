
namespace AccountRefactorWithCSharp;

public class BudgetService
{
    private readonly IBudgetRepo _budgetRepo;

    public BudgetService(IBudgetRepo budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }

    public double Query(DateTime start, DateTime end)
    {
        var budgets = _budgetRepo.GetAll();

        // inline variables: startYearMonth and endYearMonth
        // Rider 使用 Ctrl + Alt + N 會自動 inline - IntelliJ IDEA的快捷鍵配置
        var startMonthDays = DateTime.DaysInMonth(start.Year, start.Month);
        var endMonthDays = DateTime.DaysInMonth(end.Year, end.Month);

        if (start.ToString("yyyyMM") != end.ToString("yyyyMM"))
        {
            var temp = start.AddMonths(1);
            var nextMonthFirst = new DateTime(temp.Year, temp.Month, 1);
            var sum = 0;
            while (nextMonthFirst < new DateTime(end.Year, end.Month, 1))
            {
                var budget = GetBudget(budgets, $"{nextMonthFirst:yyyyMM}");
                if (budget != null) sum += budget.Amount;
                nextMonthFirst = nextMonthFirst.AddMonths(1);
            }

            var startBudget = GetBudget(budgets, start.ToString("yyyyMM"));
            var endBudget = GetBudget(budgets, end.ToString("yyyyMM"));

            var startBudgetPerDay = startBudget?.Amount / startMonthDays ?? 0;
            var endBudgetPerDay = endBudget?.Amount / endMonthDays ?? 0;

            sum += startBudgetPerDay * (startMonthDays - start.Day + 1) + endBudgetPerDay * (end.Day);
            return sum;
        }

        var oneMonthBudget = GetBudget(budgets, start.ToString("yyyyMM"));
        if (oneMonthBudget == null) return 0;

        var amount = oneMonthBudget.Amount;
        var amountPerDay = amount / startMonthDays;
        return amountPerDay * ((end - start).Days + 1);
    }

    private static Budget? GetBudget(List<Budget> budgets, string yearMonth)
    {
        return budgets.FirstOrDefault(b => b.YearMonth == yearMonth);
    }
}