"use client";
import {
  CalendarOutlined,
  DollarOutlined,
  EditOutlined,
  LeftOutlined,
  SnippetsOutlined,
  TagOutlined,
} from "@ant-design/icons";
import { Button, Checkbox, Modal, Spin } from "antd";
import { images } from "@/theme";
import Image from "next/image";
import { observer } from "mobx-react-lite";
import { Manrope } from "next/font/google";
import Link from "next/link";
import { useEffect, useState } from "react";
import { useStores } from "@/hooks/use-store";
import { useRouter } from "next/navigation";

const manRope = Manrope({
  subsets: ["latin"],
  display: "swap",
  variable: "--font-manrope",
});
interface DataType {
  rnDate: "";
  customer: {
    custAID: string;
    title: string;
    firstName: string;
    surname: string;
    othername: string;
    fullName: string;
    mobPhone: string;
    phone: string;
    email: string;
  };
  branchDetail: {
    name: "";
  };
  accoutMasBank: {
    productName: "";
  };
  subBank: {
    accountName: "";
  };
  paymentNo: "";
  amount: "";
  recNo: "";
  ref: "";
  instrumentType: "";
  chqueNo: "";
  transDesc: "";
  balance: "";
  doNotChargeBankStampDuty: boolean | undefined;
  status: string;
}
export default observer(function Page({
  params,
}: {
  params: { payment_Details: string };
}) {
  const { depositStore } = useStores();
  const router = useRouter();
  const [TransactionDetails, setTransactionDetails] = useState<DataType>({
    customer: {
      custAID: "",
      title: "",
      firstName: "",
      surname: "",
      othername: "",
      fullName: "",
      mobPhone: "",
      phone: "",
      email: "",
    },
    branchDetail: {
      name: "",
    },
    accoutMasBank: {
      productName: "",
    },
    subBank: {
      accountName: "",
    },
    paymentNo: "",
    amount: "",
    recNo: "",
    rnDate: "",
    instrumentType: "",
    chqueNo: "",
    ref: "",
    transDesc: "",
    balance: "",
    doNotChargeBankStampDuty: undefined,
    status: "",
  });
  const [isloading, setIsLoading] = useState(true);
  const [isapproveModalOpen, setisApprovedModalOpen] = useState(false);
  const [isReversedModalOpen, setIsReversedModalOpen] = useState(false);
  const [isdeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [confirmationModalOpen, setIsConfirmationModal] = useState(false);

  console.log(TransactionDetails);
  const navigateToDeposit = () => {
    router.push("/user/accounting/customer-deposit/payments");
  };
  const navigateToNewDepositPage = () => {
    router.push("/user/accounting/customer-deposit/payments/add-payment");
  };
  const navigateToEditDeposit = (paymentNo: string) => {
    router.push(`/user/accounting/customer-deposit/payments/${paymentNo}/Edit`);
  };

  const fetch = async () => {
    const fetchTransactionDetails = await depositStore.getPaymentByCode(
      params.payment_Details.toString()
    );
    // const getInstrumentTypeResponse = await depositStore.getInstrumentTYpe();
    if (fetchTransactionDetails.kind === "ok") {
      setTransactionDetails(fetchTransactionDetails.data.result);
      // setPaymentInstrument(getInstrumentTypeResponse.data.result)
      setIsLoading(false);
    } else {
      setIsLoading(false);
    }
  };

  const onSuccessActionClose = () => {
    setIsLoading(true);
    navigateToDeposit();
  };
  const approveTransactionHandler = async (code: string) => {
    setisApprovedModalOpen(false);
    setIsLoading(true);
    const res = await depositStore.approvePaymentTransaction(code);
    if (res.kind === "ok") {
      setIsLoading(false);
      setIsConfirmationModal(true);
    } else {
      setIsLoading(false);
    }
    console.log(res);
  };

  const reverseTransactionHandler = async (code: string) => {
    const res = await depositStore.reverseTransaction(code);
    if (res.kind === "ok") {
      setIsLoading(false);
      setIsReversedModalOpen(false);
      setIsConfirmationModal(true);
    } else {
      setIsLoading(false);
    }
    console.log(res);
  };

  useEffect(() => {
    fetch();
  }, []);

  const onCancelHandler = (boolean: boolean) => !boolean;

  const handleSubmit = (boolean: boolean) => !boolean;

  return (
    <div className={`${manRope.variable} w-[90%] mx-auto  flex flex-col gap-5`}>
      <Spin tip="fetching...." spinning={isloading} className="text-[red] ">
        <div className="flex flex-col gap-5 rounded-md pt-2 ">
          <div className="flex gap-12 justify-between  items-center  py-5">
            <Link href="/user/accounting/customer-deposit">
              <LeftOutlined />
            </Link>
            <div className="text-center font-[500]   capitalize text-[15px] leading-tight">
              <p className="text-[32px] text-[#1A202C] font-[800] leading-[40px]">
                {TransactionDetails?.amount.toLocaleString()}.00
              </p>
              <p className="text-[#718096] font-[500] text-[18px]">
                deposited by :{" "}
                {TransactionDetails?.customer &&
                  TransactionDetails.customer.fullName}
              </p>
              <p className="text-[#718096]">
                to{" "}
                {TransactionDetails?.customer &&
                  TransactionDetails?.customer.firstName}
              </p>
            </div>
            <span></span>
          </div>
          <div>
            <p className="flex justify-start gap-2 items-center font-bold text-[16px] leading-10 px-2 w-full capitalize bg-[#194bfb] text-white rounded-t-lg">
              <span>
                <Image
                  src={images.pin_icon}
                  alt="icon"
                  className="cursor-pointer  text-[10px] "
                />
              </span>
              Customer Deposits Details
            </p>
            {/* table data  */}
            <div className="p-8 flex flex-col gap-1 shadow-lg">
              <div className="border-2 border-[#D8E3F8] grid grid-cols-2">
                <div className="grid grid-cols-2 justify-center items-center">
                  <p className="p-2 flex justify-start items-center gap-2 font-[500] text-[16px] leading-[24px] text-[#5D6A83]">
                    <TagOutlined /> Transaction No
                  </p>
                  <p className="p-2 font-[700] text-[16px] leading-[24px] text-[#2A313C] bg-[#D8E3F8]">
                    {TransactionDetails?.paymentNo || "null"}
                  </p>
                </div>
                <div className="grid grid-cols-2 justify-center items-center">
                  <p className="p-2 flex justify-start items-center gap-2 font-[500] text-[16px] leading-[24px] text-[#5D6A83]">
                    <TagOutlined /> Receipt No
                  </p>
                  <p className="p-2 font-[700] text-[16px] leading-[24px] text-[#2A313C] bg-[#D8E3F8]">
                    {TransactionDetails?.recNo || "null"}
                  </p>
                </div>
              </div>

              <div className="border-2 border-[#D8E3F8] grid grid-cols-2">
                <div className="grid grid-cols-2 justify-center items-center">
                  <p className="p-2 flex justify-start items-center gap-2 font-[500] text-[16px] leading-[24px] text-[#5D6A83]">
                    <TagOutlined /> Transaction Date
                  </p>
                  <p className="p-2 font-[700] text-[16px] leading-[24px] text-[#2A313C] bg-[#D8E3F8]">
                    {TransactionDetails?.rnDate.slice(0, 10) || "null"}
                  </p>
                </div>
                <div className="grid grid-cols-2 justify-center items-center">
                  <p className="p-2 flex justify-start items-center gap-2 font-[500] text-[16px] leading-[24px] text-[#5D6A83]">
                    <TagOutlined /> Branch A/C
                  </p>
                  <p className="p-2 font-[700] text-[16px] leading-[24px] text-[#2A313C] bg-[#D8E3F8]">
                    {TransactionDetails?.branchDetail?.name || "null"}
                  </p>
                </div>
              </div>

              <div className="border-2 border-[#D8E3F8] grid grid-cols-2">
                <div className="grid grid-cols-2 justify-center items-center">
                  <p className="p-2 flex justify-start items-center gap-2 font-[500] text-[16px] leading-[24px] text-[#5D6A83]">
                    <TagOutlined /> Product A/C
                  </p>
                  <p className="px-2 h-full font-[700] flex justify-start items-center text-[16px] leading-[24px] text-[#2A313C] bg-[#D8E3F8]">
                    <span>
                      {TransactionDetails?.accoutMasBank?.productName}
                    </span>
                  </p>
                </div>
                <div className="grid grid-cols-2 justify-center items-center">
                  <p className="px-2 flex justify-start items-center gap-2 font-[500] text-[16px] leading-[24px] text-[#5D6A83]">
                    <TagOutlined /> Subsidiary A/C
                  </p>
                  <p className="px-2 font-[700] text-[16px] leading-[24px] text-[#2A313C] py-2 bg-[#D8E3F8]">
                    {TransactionDetails?.customer?.fullName}
                  </p>
                </div>
              </div>

              <div className="border-2 border-[#D8E3F8] grid grid-cols-2">
                <div className="grid grid-cols-2 justify-center items-center">
                  <p className="px-2 flex justify-start items-center gap-2 font-[500] text-[16px] leading-[24px] text-[#5D6A83]">
                    <TagOutlined /> Bank A/C
                  </p>
                  <p className="px-2 font-[700] text-[16px] leading-[24px] text-[#2A313C] bg-[#D8E3F8]">
                    {TransactionDetails?.subBank.accountName}
                  </p>
                </div>
                <div className="grid grid-cols-2 justify-center items-center">
                  <p className="px-2 flex justify-start h-full items-center gap-2 font-[500] text-[16px] leading-[24px] text-[#5D6A83]">
                    <TagOutlined /> Amount Paid
                  </p>
                  <p className="px-2 h-full font-[700] flex justify-start items-center text-[16px] leading-[24px] text-[#2A313C] bg-[#D8E3F8]">
                    <span>N{TransactionDetails?.amount.toLocaleString()}</span>
                  </p>
                </div>
              </div>

              <div className="border-2 border-[#D8E3F8] grid grid-cols-2">
                <div className="grid grid-cols-2 justify-center items-center">
                  <p className="p-2 flex justify-start items-center gap-2 font-[500] text-[16px] leading-[24px] text-[#5D6A83]">
                    <TagOutlined /> Paying Instr.
                  </p>
                  <p className="p-2 font-[700] text-[16px] leading-[24px] text-[#2A313C] bg-[#D8E3F8]">
                    {TransactionDetails?.instrumentType}
                  </p>
                </div>
                <div className="grid grid-cols-2 justify-center items-center">
                  <p className="p-2 flex justify-start items-center gap-2 font-[500] text-[16px] leading-[24px] text-[#5D6A83]">
                    <TagOutlined /> Cheque No.
                  </p>
                  <p className="p-2 font-[700] text-[16px] leading-[24px] text-[#2A313C] bg-[#D8E3F8]">
                    {TransactionDetails?.chqueNo || "null"}
                  </p>
                </div>
              </div>

              <div className="border-2 border-[#D8E3F8] grid grid-cols-2">
                <div className="grid grid-cols-2 justify-center items-center">
                  <p className="p-2 flex justify-start items-center gap-2 font-[500] text-[16px] leading-[24px] text-[#5D6A83]">
                    <TagOutlined /> Reference No.
                  </p>
                  <p className="p-2 font-[700] text-[16px] leading-[24px] text-[#2A313C] bg-[#D8E3F8]">
                    {TransactionDetails?.ref}
                  </p>
                </div>
                <div className="grid grid-cols-2 justify-center items-center">
                  <p className="p-2 flex justify-start items-center gap-2 font-[500] text-[16px] leading-[24px] text-[#5D6A83]">
                    <TagOutlined /> Narration
                  </p>
                  <p className="p-2 font-[700] text-[16px] leading-[24px] text-[#2A313C] bg-[#D8E3F8]">
                    {TransactionDetails?.transDesc || "Empty"}
                  </p>
                </div>
              </div>

              <div className="flex justify-between items-center mt-5  ">
                <p className="text-[#128A3E] font-[700] text-[16px] leading-[20px] ">{`Email: ${
                  TransactionDetails?.customer?.email
                } | Phone: ${
                  TransactionDetails.customer?.phone ??
                  TransactionDetails.customer?.mobPhone
                } | Cash Balance: N${TransactionDetails?.balance?.toLocaleString()}`}</p>
                <Checkbox
                  checked={TransactionDetails?.doNotChargeBankStampDuty}
                >
                  <p className="font-bold text-[16px] leading-[20px]">
                    Do not charge Bank Stamp Duty
                  </p>
                </Checkbox>
              </div>

              {/* buttons   */}
              <div className="flex justify-between items-center gap-3 mt-5">
                <div className="flex justify-between items-center gap-2">
                  <Button className=" inline-flex font-[700] text-[#DD3333] capitalize bg-[#FEE2E2]  border-0 py-2 px-4 focus:outline-none hover:bg-[#FEE2E2] rounded text-[16px]">
                    Delete Deposit
                  </Button>
                  <button
                    type="button"
                    className=" inline-flex font-[700] capitalize text-[#194BFB]  bg-[#EDF2F7] border-0 py-2 px-4 focus:outline-none hover:bg-[#d7d8da] rounded text-[16px]"
                    onClick={() => navigateToDeposit()}
                  >
                    view Unapproved deposit
                  </button>
                  <button
                    type="button"
                    className=" inline-flex font-[700] capitalize text-[#194BFB]  bg-[#EDF2F7] border-0 py-2 px-4 focus:outline-none hover:bg-[#d7d8da] rounded text-[16px]"
                    onClick={() => navigateToNewDepositPage()}
                  >
                    Add New Deposit
                  </button>

                  {TransactionDetails?.status === "UnPosted" ? (
                    <button
                      type="button"
                      className=" inline-flex font-[700] capitalize text-[#194BFB]  bg-[#EDF2F7] border-0 py-2 px-4 focus:outline-none hover:bg-[#d7d8da] rounded text-[16px]"
                      onClick={() =>
                        navigateToEditDeposit(TransactionDetails?.paymentNo)
                      }
                    >
                      Edit Deposit
                    </button>
                  ) : null}
                </div>
                <div>
                  {TransactionDetails?.status === "UnPosted" ? (
                    <Button
                      className=" inline-flex font-[700] capitalize text-[#d7d8da]  bg-[#194BFB] border-0 py-2 px-4 focus:outline-none hover:bg-[#194BFB] rounded text-[16px]"
                      onClick={() => setisApprovedModalOpen(true)}
                    >
                      Approve
                    </Button>
                  ) : TransactionDetails?.status === "Posted" ? (
                    <Button
                      className=" inline-flex font-[700] capitalize text-[#d7d8da]  bg-[#194BFB] border-0 py-2 px-4 focus:outline-none hover:bg-[#194BFB] rounded text-[16px]"
                      onClick={() => setIsReversedModalOpen(true)}
                    >
                      Reversed
                    </Button>
                  ) : null}
                </div>
              </div>
            </div>

            <div className="flex justify-between items-center ">
              <p className="font-[600] flex gap-2 items-center text-[16px] leading-[24px] p-8">
                <DollarOutlined />
                Status
              </p>
              <Button className=" inline-flex font-[700] text-[#FF784B] capitalize bg-[#FEE2E2]  border-0 py-2 px-4 focus:outline-none hover:bg-[#FEE2E2] rounded text-[16px]">
                {TransactionDetails?.status}
              </Button>
            </div>
          </div>
        </div>
      </Spin>

      {/* approved modal  */}
      <Modal
        open={isapproveModalOpen}
        centered={true}
        okText="save"
        cancelText="Make Changes"
        onOk={() => approveTransactionHandler(TransactionDetails.paymentNo)}
        onCancel={() => setisApprovedModalOpen(!isapproveModalOpen)}
        footer={[
          <div key={2} className="  flex justify-center items-center">
            <Button
              key="submit"
              type="primary"
              className=" text-[16px] text-center w-[30%]  font-[500]"
              onClick={() =>
                approveTransactionHandler(TransactionDetails.paymentNo)
              }
            >
              Approve
            </Button>
          </div>,
        ]}
      >
        <div className="py-3 flex flex-col justify-center items-center text-center">
          <Image src={images.warningModalImage} alt="warning_Modal_Image" />
          <p className="text-[20px] leading-[26px] font-bold w-[80%] mx-auto">
            Are you sure you want to approve the transaction ?
          </p>
        </div>
      </Modal>
      {/* Reversed Modal  */}
      <Modal
        open={isReversedModalOpen}
        centered={true}
        okText="save"
        cancelText="Make Changes"
        onOk={() => reverseTransactionHandler(TransactionDetails.paymentNo)}
        onCancel={() => onCancelHandler(isReversedModalOpen)}
        footer={[
          <div key={7} className="  flex justify-center items-center">
            <Button
              key="submit"
              type="primary"
              className=" text-[16px] text-center w-[30%]  font-[500]"
              onClick={() =>
                reverseTransactionHandler(TransactionDetails.paymentNo)
              }
            >
              Reversed
            </Button>
          </div>,
        ]}
      >
        <div className="w-[80%] mx-auto justify-center items-center text-center mt-8 gap-5">
          <Image src={images.warningModalImage} alt="warning_Modal_Image" />
          <p className="text-[20px] leading-[26px] font-bold capitalize">
            Are you sure you want to reverse the transaction?
          </p>
        </div>
      </Modal>
      {/* delete modal  */}
      <Modal
        open={isdeleteModalOpen}
        centered={true}
        okText="save"
        cancelText="Make Changes"
        onOk={() => setIsDeleteModalOpen(!isdeleteModalOpen)}
        onCancel={() => setIsDeleteModalOpen(false)}
        footer={[
          <div key={2} className="  flex justify-center items-center">
            <Button
              key="submit"
              // type="primary"
              className=" text-[16px] text-center w-[30%] bg-[#FF4747] text-white font-[500]"
              onClick={() => setIsDeleteModalOpen(false)}
            >
              Delete
            </Button>
          </div>,
        ]}
      >
        <div className="flex flex-col justify-center items-center text-center  p-4 ">
          <Image src={images.warningModalImage} alt="warning_Modal_Image" />
          <p className="text-[20px] leading-[26px] font-bold ">
            Are you sure you want to delete the transaction?
          </p>
        </div>
      </Modal>

      {/* modal for sucessfully Approved  */}
      <Modal
        open={confirmationModalOpen}
        onCancel={() => onSuccessActionClose()}
        centered={true}
        footer={[
          <div
            key={4}
            className=" w-full flex justify-center items-center capitalize"
          >
            <p className="text-[20px] font-bold leading-[26px] text-center capitalize">
              Transaction with ID : {TransactionDetails?.paymentNo} Sucessfully
            </p>
          </div>,
        ]}
      >
        <div className="w-[80%] mx-auto justify-center items-center text-center mt-8 gap-5">
          <Image src={images.successModalImage} alt="warning_Modal_Image" />
        </div>
      </Modal>
    </div>
  );
});
