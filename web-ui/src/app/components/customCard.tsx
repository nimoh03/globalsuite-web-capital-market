export const CustomCard = ({ headerContent, cardClassname, children }: any) => {
  return (
    <div
      className={`rounded-lg overflow-hidden shadow-xl max-w-[900px] border border-gray-400 ${cardClassname}`}
    >
      {headerContent && (
        <div className="flex justify-start gap-2 items-center font-bold text-[16px] leading-10 px-2 w-full capitalize bg-[#194bfb] text-white ">
          {headerContent}
        </div>
      )}

      {children}
    </div>
  );
};
