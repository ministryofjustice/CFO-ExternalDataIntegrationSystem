namespace Matching.Engine.Scoring.Validators;

public interface IBayesValidator
{
    public bool IsValid(out string[] requiredFields);
}
