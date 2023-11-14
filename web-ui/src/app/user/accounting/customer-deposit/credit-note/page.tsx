"use client";

import { useStores } from "@/hooks/use-store";
import { CaretDownOutlined, SearchOutlined } from "@ant-design/icons";
import { Button, Input, Modal, Select, Spin, Table, Tooltip } from "antd";
import { observer } from "mobx-react-lite";
import { useEffect, useState } from "react";
import type { ColumnsType } from "antd/es/table";
import Link from "next/link";
import { images } from "@/theme";
import Image from "next/image";
import { useRouter } from "next/navigation";
import { BtnSectionInTable } from "@/helpers/helper";
interface MyData {
  creditNo: string;
}
export default observer(
  function AccountingCustomerDepositCustomerOpeningBalance() {
    const { depositStore } = useStores();
    const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>([]);
    const [isloading, setIsloading] = useState(true);
    const [isOpen, setIsOpen] = useState(false);
    const [creditNoteStatus, setCreditNoteStatus] = useState("UnPosted");
    const [tableData, setTableData] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isModalOpen2, setIsModalOpen2] = useState(false);
    const [selectedRowData, setSelectedRowData] = useState<MyData[]>([]);
    const [totalTransactionRecord, setTotalTransactionRecord] = useState(0);
    const router = useRouter();
const navigateToNewDepositPage = () => {
      router.push(`/user/accounting/customer-deposit/credit-note/Create_New_Credit_Note`);
    }; 
    const navigateToDetailsPage = (creditNo: String) => {
      router.push(`/user/accounting/customer-deposit/credit-note/${creditNo}`);
    };
    const navigateToEditDeposit = (paymentNo: string) => {
      router.push(`/user/accounting/customer-deposit/credit-note/${paymentNo}/Edit`);
    };
    interface DataType {
      key: React.Key;
      TxnID: string;
      account: number;
      amount: number;
      status: string;
      createdBY: string;
      date: string;
      dataSource: [];
      creditNo: string;
    }

    const columns: ColumnsType<DataType> = [
      {
        title:<p className="font-[800] text-[16px] leading-[24px] text-center">Date</p>,
        responsive: ["sm"],
        width: 80,
        dataIndex: "rnDate",
        key: "rnDate",
        render: (text: String) => <p>{text.slice(0, 10)}</p>,
      },
      {
        title: <p className="font-[800] text-[16px] leading-[24px] text-center">Customer</p>,
        responsive: ["sm"],
        width: 300,
         dataIndex: "customer",
        key: "customer",
        sorter: true,
        render: (text) => (
          <p className="text-[14px] font-[600] leading-[18.2px]">{text.fullName.replace('.','')} - {text.custAID}</p>
       ),
      },
      {
        title: <p className="font-[800] text-[16px] leading-[24px] text-center">Amount</p>,
        responsive: ["sm"],
        width: 80,
        dataIndex: "amount",
        key: "amount",
        sorter: (a: any, b: any) => a.amount - b.amount,
        render: (text) => (
          <p className="capitalize font-[600] text-[14px]"> {text.toLocaleString()}</p>
        ),
      },
      {
        title: <p className="font-[800] text-[16px] leading-[24px] text-center uppercase">Txn ID</p>,
        responsive: ["sm"],
        width: 100,
        dataIndex: "creditNo",
        key: "creditNo",
        sorter: (a: any, b: any) => a.creditNo.length - b.creditNo.length,
        render: (text) => (
          <p className="capitalize text-[14px]">{text}</p>
        )
      },

    {
      title: <p className="font-[800] text-[16px] leading-[24px] text-center">Created By</p>,
      responsive: ["sm"],
      width: 120,
       dataIndex: "source",
      key: "source",
      render: (text) => (
        <p className="capitalize text-[14px]">{text.toLowerCase()}</p>
      )
    },

      {
        title:<p className="font-[800] text-[16px] leading-[24px] text-center">Actions</p>,
        key: "action",
        responsive: ["sm"],
        width: 100,
        render: (record) => (
          <div className="flex justify-evenly items-center gap-2">
            <Tooltip placement="bottom" title="approve">
              <Image
                src={images.checkIncon}
                alt="icon"
                className="cursor-pointer w-[15%] text-[10px] text-white"
                onClick={() => navigateToDetailsPage(record.creditNo)}
              />
            </Tooltip>
            
            <Tooltip placement="bottom" title="edit">
              <Image
                src={images.pencil_icon}
                alt="icon"
                className="cursor-pointer w-[15%] text-[10px] "
                onClick={() => navigateToEditDeposit(record.creditNo)}
              />
            </Tooltip>
            <Tooltip placement="bottom" title="delete">
              <Image
                src={images.deleteIcon}
                alt="icon"
                className="cursor-pointer w-[15%] text-[10px] "
                onClick={() => navigateToDetailsPage(record.creditNo)}
              />
            </Tooltip>
          </div>
        ),
      },
    ];
  
    const onSelectChange = (
      newSelectedRowKeys: any[],
      selectedRows: DataType[]
    ) => {
      console.log("selectedRowKeys changed: ", newSelectedRowKeys);
      setSelectedRowKeys(newSelectedRowKeys);
      setSelectedRowData(selectedRows);
    };
    const rowSelection = {
      selectedRowKeys,
      onChange: onSelectChange,
    };

    const hasSelected = selectedRowKeys.length > 0;
    const handleCancel = () => {
      setIsModalOpen(false);
      setIsloading(false);
    };
    const batchApproveHandler = (arr: { creditNo: string }[]) => {
      setIsModalOpen(false)
      setTotalTransactionRecord(arr.length );
      setIsloading(true);
      arr.map(async (value) => {
        const res = await depositStore.approveCreditNoteTransaction(value.creditNo);
        if (res.kind === "ok") {
          setIsModalOpen(false);
          setIsloading(false);
          setIsModalOpen2(true);
        } else {
          setIsloading(false);
        }
      });
    };
    const saveStatusHandler=(status:string)=>{
        depositStore.setCreditNoteStatus(status)
    }

    const fetch = async () => {
      saveStatusHandler(creditNoteStatus)
      setIsloading(true);

      const fetchTableData = await depositStore.getAllCreditNote(
        creditNoteStatus
      );
      if (fetchTableData.kind === "ok") {
        const mappedData = fetchTableData.data.result.map(
          (value: Object, index: number) => ({
            ...value,
            key: index.toString(),
          })
        );
        setTableData(mappedData);
        setIsloading(false);
      } else {
        setIsloading(false);
        setTableData([]);
      }
    };

    useEffect(() => {
      fetch();
    }, [creditNoteStatus]);
    return (
      <>
      <Spin tip="fetching...." spinning={isloading} className="text-[red] ">
        <main className=" min-h-screen flex flex-col gap-1 w-[95%]  mx-auto ">
          
        <div className="flex justify-end items-center gap-2">
            {hasSelected && (
              <div className="flex justify-between items-between gap-2 ">
                <Button className=" inline-flex font-[700] text-[#DD3333] capitalize bg-[#FEE2E2]  border-0 py-2 px-4 focus:outline-none hover:bg-[#FEE2E2] rounded text-[16px]">
                  Batch Delete
                </Button>
                <Button
                  className=" inline-flex font-[700] capitalize text-[#d7d8da]  bg-[#194BFB] border-0 py-2 px-4 focus:outline-none hover:bg-[#194BFB] rounded text-[16px]"
                  onClick={() => setIsModalOpen(true)}
                >
                  Batch Approve
                </Button>
              </div>
            )}

            <button
              type="button"
              className=" flex gap-3 justify-center items-center font-[500] capitalize  border-0 py-2 px-4 focus:outline-none hover:bg-[#d7d8da] rounded text-[16px] leading-[24px]"
              onClick={() => setIsOpen(!isOpen)}
            >
              filters <CaretDownOutlined />
            </button>
            <button
              type="button"
              className=" inline-flex font-[700] capitalize text-[#194BFB]  bg-[#EDF2F7] border-0 py-2 px-4 focus:outline-none hover:bg-[#d7d8da] rounded text-[16px]"
              onClick={() => setCreditNoteStatus("Reversed")}
            >
              view reversed deposit
            </button>
            <button
              type="button"
              className=" inline-flex font-[700] capitalize text-[#194BFB]  bg-[#EDF2F7] border-0 py-2 px-4 focus:outline-none hover:bg-[#d7d8da] rounded text-[16px]"
              onClick={() => setCreditNoteStatus("Posted")}
            >
              view Approved deposit
            </button>
            <button
              type="button"
              className="inline-flex font-[700] capitalize text-white  bg-[#194BFB] border-0 py-2 px-6 focus:outline-none hover:bg-[#191dfb] rounded text-[16px]"
              onClick={() => navigateToNewDepositPage()}
            >
              Add New Deposit
            </button>
          </div>
          <div className=" rounded-lg shadow-xl w-full ">
            <p className="flex justify-start gap-2 items-center font-bold text-[16px] leading-10 px-2 w-full capitalize bg-[#194bfb] text-white rounded-t-lg">
              <span>
                <Image
                  src={images.pin_icon}
                  alt="icon"
                  className="cursor-pointer  text-[10px] "
                />
              </span>
              {creditNoteStatus === "Unposted"
                ? "Unapproved"
                : creditNoteStatus === "Posted"
                ? "Approved"
                : creditNoteStatus === "Reversed"
                ? "Reversed"
                : null}{" "}
              Customer Credits
            </p>
            <Table
              sticky={{
                offsetHeader: 50,
              }}
              size="small"
              scroll={{ x: "max-content" }}
              rowSelection={rowSelection}
              columns={columns}
              dataSource={tableData}
              bordered
            />
          </div>
        </main>
      </Spin>
        <Modal
        open={isModalOpen}
        centered={true}
        okText="save"
        cancelText="Make Changes"
        onOk={() => batchApproveHandler(selectedRowData)}
        onCancel={handleCancel}
        footer={[
          <div key={2} className="  flex justify-center items-center">
            <Button
              key="submit"
              type="primary"
              className="w-[50%] font-[700] text-[16px] leading-[16px]"
              onClick={() => batchApproveHandler(selectedRowData)}
            >
              Approve
            </Button>
          </div>,
        ]}
      >
        <div className="mx-auto w-[90%] justify-center items-center text-center mt-8 gap-5">
          <div className="h-[155px]  ">
            <Image
              src={images.warningModalImage}
              alt="warning_Modal_Image"
              className="object-cover  w-full"
            />
          </div>
          <p className="font-[800] text-[20px] text-center leading-[26px] capitalize">
            Are you sure you want to approve all the transaction?
          </p>
        </div>
      </Modal>
      {/* modal for sucessfully saved deposit */}
      <Modal
        open={isModalOpen2}
        onCancel={() => window.location.reload()}
        centered={true}
        footer={[
          <div
            key={4}
            className=" w-full flex flex-col justify-center items-center capitalize"
          >
            <p className="text-[20px] font-bold leading-[26px] text-center capitalize">
              {`${totalTransactionRecord} Transaction approved successfully`}
            </p>
          </div>
        ]}
      >
        <div className="w-[90%] mx-auto flex flex-col justify-center items-center text-center mt-8 h-[155px] ">
          <Image src={images.successModalImage} alt="warning_Modal_Image" />
        </div>
      </Modal>
      </>
    );
  }
);
