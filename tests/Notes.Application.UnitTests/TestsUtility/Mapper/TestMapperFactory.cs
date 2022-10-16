using AutoMapper;

namespace Notes.Application.UnitTests.TestsUtility.Mapper;

public static class TestMapperFactory
{
    public static IMapper GetTestMapper()
    {
        var myProfile = new TestNoteProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        return new AutoMapper.Mapper(configuration);
    }
}