import moment from "moment";
import { FormInstance } from "antd";
import { CaretDownOutlined } from "@ant-design/icons";
import { useStores } from "@/hooks/use-store";
import DepositStore from "@/stores/DepositStore";
interface BtnSectionProps {
  setIsOpen: (value: boolean) => void;
  isOpen: boolean;
  navigateToNewDepositPage: () => void;
}
interface setState {
  (value: boolean): void;
}

const currentDate = new Date();

export const DefaultTime = moment(currentDate);

export const onSuccessDepositClose = (
  setIsModalOpen2: setState,
  setIsLoading: setState,
  handleReset: () => void
) => {
  handleReset();
  setIsModalOpen2(false);
  setIsLoading(false);
};

export const onChangeProduct = (
  form: FormInstance,
  setallSubBank: (value: {
    key:string;
    label: string;
    value: string;
}[]) => void,
  values: {}
) => {
  setallSubBank([{key: '', label: "", value: "" }]);
  form.setFieldsValue({
    ...values,
    custNo: "",
  });
};

export const BtnSectionInTable = ({
  setIsOpen,
  isOpen,
  navigateToNewDepositPage,
}: BtnSectionProps) => {
  return (
    <>
      <div className="flex justify-end items-end mb-3 gap-5">
        <button
          type="button"
          className=" text-[grey] capitalize flex justify-center   items-center gap-2 border py-1 px-3 focus:outline-none rounded text-md"
          onClick={() => setIsOpen(!isOpen)}
        >
          filters <CaretDownOutlined />
        </button>

        <button
          type="button"
          className="inline-flex text-white  bg-[#194BFB] border-0 py-2 px-6 focus:outline-none hover:bg-[#191dfb] rounded text-md"
          onClick={() => navigateToNewDepositPage()}
        >
          Add New Deposit
        </button>
      </div>
    </>
  );
};

export const fetch = async (
  setPaymentInstrument: (value: []) => void,
  setAllBranched: (value: []) => void,
  setAllProduct: (value: []) => void,
  form: FormInstance,
  values: {},
  setIsLoading: setState,
  depositStore: DepositStore,
  setDefaultBranch:(value:string)=>void
) => {
  setIsLoading(true);
  const getInstrumentTypeResponse = await depositStore.getInstrumentTYpe();
  const getAllBranchResponse = await depositStore.getAllBranch();

  const getAllProductResponse = await depositStore.getAllProducts();

  if (
    getInstrumentTypeResponse.kind === "ok" &&
    getAllBranchResponse.kind === "ok" &&
    getAllProductResponse.kind === "ok"
  ) {
    setPaymentInstrument(getInstrumentTypeResponse.data.result);

    setAllBranched(getAllBranchResponse.data.result);
    setAllProduct(getAllProductResponse.data.result);
    form.setFieldsValue({
      ...values,
      acctMasBank: getAllProductResponse.data.result[0].productCode,
      branch: getAllBranchResponse.data.result[0].transNo,
    });
    setDefaultBranch(getAllBranchResponse.data.result[0].transNo,)
    setIsLoading(false);
  } else {
    setIsLoading(false);
  }
};
