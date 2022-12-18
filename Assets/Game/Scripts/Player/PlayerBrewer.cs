using UnityEngine;

public class PlayerBrewer : MonoBehaviour
{
    [SerializeField] private float brewingRange = 2.5f;
    [SerializeField] private LayerMask brewingPotMask;
    [SerializeField] private IngredientList ingredientList;
    private BuildingConstructor buildingConstructor;

    private void Awake()
    {
        buildingConstructor = GetComponent<BuildingConstructor>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, brewingRange);
    }

    public void AttemptBrew(out bool success)
    {
        success = false;

        if (ingredientList.value.Count < 3)
            return;

        foreach (var brewingPotCollider in Physics.OverlapSphere(transform.position, brewingRange, brewingPotMask))
            if (brewingPotCollider.TryGetComponent(out BrewingPot brewingPot))
            {
                print("Found Pot!");
                var recipe = brewingPot.GetFittingRecipe(ingredientList.value);

                if (recipe != null)
                {
                    print("Brewed successfully!");
                    buildingConstructor.currentBuilding = brewingPot.Brew(ref ingredientList.value, recipe);
                    success = true;
                }
            }
    }
}