namespace master.framework.database
{
    public static class MappingEngine
    {
        private static bool isMapped { get; set; }
        public static void Map()
        {
            if (!isMapped)
            {
                AutoMapper.Mapper.Reset();
                isMapped = true;
            }
        }
        public static void Reset()
        {
            isMapped = false;
        }

        public static void Mapper<TSource, TDestination>()
        {
            var exist = AutoMapper.Mapper.FindTypeMapFor<TSource, TDestination>();
            if (exist == null) { AutoMapper.Mapper.CreateMap<TSource, TDestination>().ReverseMap(); }
        }
    }
}
