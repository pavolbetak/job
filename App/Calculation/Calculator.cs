namespace App.Calculation
{
    public class Calculator
    {
        public double CalculateItemData(double value)
        {
            return Math.Pow(Math.Log(value), 3);
        }
    }
}
