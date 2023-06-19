
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

        // 把宣告變數的位置搬到離使用它最近的地方，補上 else 的區塊，避免相同變數名字導致問題

        if (start.ToString("yyyyMM") != end.ToString("yyyyMM"))
        {
            var currentMonth = new DateTime(start.Year, start.Month, 1).AddMonths(1);
            var sum = 0;
            while (currentMonth < new DateTime(end.Year, end.Month, 1))
            {
                var budget = GetBudget(budgets, $"{currentMonth:yyyyMM}");
                if (budget != null)
                {
                    sum += budget.Amount;
                }
                currentMonth = currentMonth.AddMonths(1);
            }

            var startBudget = GetBudget(budgets, start.ToString("yyyyMM"));
            var startMonthDays = DateTime.DaysInMonth(start.Year, start.Month);
            // 把語法糖改成 if condition
            // Step 2
            // Rider 中在 startBudget != null ? 在?的地方 Alt + Enter，選擇 Convert to 'if' statement
            int startBudgetPerDay;
            if (startBudget != null)
                startBudgetPerDay = startBudget.Amount / startMonthDays ?? 0;
            else
                startBudgetPerDay = 0;
            var amountOfStart = startBudgetPerDay * (startMonthDays - start.Day + 1);

            var endBudget = GetBudget(budgets, end.ToString("yyyyMM"));
            var endMonthDays = DateTime.DaysInMonth(end.Year, end.Month);
            var endBudgetPerDay = endBudget?.Amount / endMonthDays ?? 0;
            var amountOfEnd = endBudgetPerDay * (end.Day);

            sum += amountOfStart + amountOfEnd;
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