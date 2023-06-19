
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
                        // 移除重複的 budget null 檢查
                        var startMonthDays = DateTime.DaysInMonth(start.Year, start.Month);
                        int startBudgetPerDay;
                        // Rider 會警告 Expression is always true  => (budget != null)
                        // Alt + Enter => replace 'if' statement with respective branch - IntelliJ IDEA的快捷鍵配置
                        startBudgetPerDay = budget.Amount / startMonthDays;

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