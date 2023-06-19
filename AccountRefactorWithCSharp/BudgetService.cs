
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
            while (currentMonth < new DateTime(end.Year, end.Month, 1))
            {
                var budget = GetBudget(budgets, currentMonth.ToString("yyyyMM"));
                if (budget != null)
                {
                    if (currentMonth.ToString("yyyyMM") == start.ToString("yyyyMM"))
                    {
                        var startBudget = GetBudget(budgets, start.ToString("yyyyMM"));
                        var startMonthDays = DateTime.DaysInMonth(start.Year, start.Month);
                        int startBudgetPerDay;
                        if (startBudget != null)
                        {
                            startBudgetPerDay = startBudget.Amount / startMonthDays;
                        }
                        else
                        {
                            startBudgetPerDay = 0;
                        }

                        var amountOfStart = startBudgetPerDay * (startMonthDays - start.Day + 1);
                        sum += amountOfStart;
                    }
                    else
                    {
                        sum += budget.Amount;
                    }
                }
                currentMonth = currentMonth.AddMonths(1);
            }

            var endBudget = GetBudget(budgets, end.ToString("yyyyMM"));
            var endMonthDays = DateTime.DaysInMonth(end.Year, end.Month);
            int endBudgetPerDay;
            if (endBudget != null)
            {
                endBudgetPerDay = endBudget.Amount / endMonthDays;
            }
            else
            {
                endBudgetPerDay = 0;
            }
            var amountOfEnd = endBudgetPerDay * (end.Day);

            sum += amountOfEnd;
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