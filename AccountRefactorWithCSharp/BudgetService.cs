
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

        var startYearMonth = start.ToString("yyyyMM");
        var endYearMonth = end.ToString("yyyyMM");
        var startMonthDays = DateTime.DaysInMonth(start.Year, start.Month);
        var endMonthDays = DateTime.DaysInMonth(end.Year, end.Month);

        if (startYearMonth != endYearMonth)
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

            var startBudget = GetBudget(budgets, startYearMonth);
            var endBudget = GetBudget(budgets, endYearMonth);

            var startBudgetPerDay = startBudget?.Amount / startMonthDays ?? 0;
            var endBudgetPerDay = endBudget?.Amount / endMonthDays ?? 0;

            sum += startBudgetPerDay * (startMonthDays - start.Day + 1) + endBudgetPerDay * (end.Day);
            return sum;
        }

        var oneMonthBudget = GetBudget(budgets, startYearMonth);
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