using AutoMapper;

namespace Notes.Application.UnitTests.MapperUtility;

public static class TestMapperFactory
{
    public static IMapper GetTestMapper()
    {
        var myProfile = new TestNoteProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        return new Mapper(configuration);
    }
}