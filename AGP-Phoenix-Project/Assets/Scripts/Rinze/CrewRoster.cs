public static class CrewRoster
{
    public static Crew[] SelectedCrew { get; private set; } = new Crew[5];

    public static void Save(Crew[] crew)
    {
        SelectedCrew = crew;
    }
}