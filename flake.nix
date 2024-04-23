{
  description = ".NET dev environment";

  inputs = {
      dotnet-env.url = "github:kijjuy/nix-flakes?dir=dotnet";
  };

  outputs = { self, dotnet-env }: dotnet-env.outputs; 
}
