
namespace Blocking.ConfigurationModels;

public class DatabaseTable
{
	public string DatabaseName { get; init; } = string.Empty;
	public string TableName { get ; init; } = string.Empty; 
	public string? Columns { get; init; }

	public string FormulateExpression => $"{DatabaseName}.{TableName}";

	public override bool Equals(object? obj)
	{
		DatabaseTable castObj;
		try
		{
			castObj = (DatabaseTable)obj;
		}
		catch (InvalidCastException ex)
		{
			return false;
		}

		if(obj == null)
		{
			return false;
		}
		else if(this.FormulateExpression == castObj.FormulateExpression && this.Columns == castObj.Columns)
		{
			return true;
		}
		return false;
	}
}
