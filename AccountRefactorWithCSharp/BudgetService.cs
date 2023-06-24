
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

        if (start.ToString("yyyyMM") != end.ToString("yyyyMM"))
        {
            var currentMonth = start;
            var sum = 0;
            while (currentMonth < new DateTime(end.Year, end.Month, 1).AddMonths(1))
            {
                var budget = GetBudget(budgets, currentMonth.ToString("yyyyMM"));
                if (budget != null)
                {
                    if (currentMonth.ToString("yyyyMM") == start.ToString("yyyyMM"))
                    {
                        var startMonthDays = DateTime.DaysInMonth(start.Year, start.Month);
                        var dailyAmount = budget.Amount / startMonthDays;

                        // 選擇 (startMonthDays - start.Day + 1)
                        // Alt + Enter => Introduce variable
                        var overlappingDays = (startMonthDays - start.Day + 1);
                        var amountOfStart = dailyAmount * overlappingDays;
                        sum += amountOfStart;
                    }
                    else if(currentMonth.ToString("yyyyMM") == end.ToString("yyyyMM"))
                    {
                        var endMonthDays = DateTime.DaysInMonth(end.Year, end.Month);
                        var dailyAmount = budget.Amount / endMonthDays;

                        // 選擇 (end.Day)
                        // Alt + Enter => Introduce variable
                        var overlappingDays = (end.Day);
                        var amountOfEnd = dailyAmount * overlappingDays;
                        sum += amountOfEnd;
                    }
                    else
                    {
                        sum += budget.Amount;
                    }
                }
                currentMonth = currentMonth.AddMonths(1);
            }
            return sum;
        }
        else
        {
            var oneMonthBudget = GetBudget(budgets, start.ToString("yyyyMM"));
            if (oneMonthBudget == null) return 0;

            var amount = oneMonthBudget.Amount;
            var startMonthDays = DateTime.DaysInMonth(start.Year, start.Month);
            var amountPerDay = amount / startMonthDays;
            return amountPerDay * ((end - start).Days + 1);
        }
    }

    private static Budget? GetBudget(List<Budget> budgets, string yearMonth)
    {
        return budgets.FirstOrDefault(b => b.YearMonth == yearMonth);
    }
}