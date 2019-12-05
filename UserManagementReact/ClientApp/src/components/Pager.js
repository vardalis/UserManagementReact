import React, { Component } from 'react';
import { Pagination, PaginationItem, PaginationLink } from 'reactstrap';

export class Pager extends Component {
	constructor(props) {
		super(props);
	}

	static defaultProps = {
		page: 1,
		totalItems: 100,
		pageSize: 10,
		maxPages: 5
	}

	componentDidMount() {
	}

	paginate() {
		const { totalItems, page, pageSize, maxPages } = this.props;
		let currentPage = page;

		// calculate total pages
		let totalPages = Math.ceil(totalItems / pageSize);

		// ensure current page isn't out of range
		if (currentPage < 1) {
			currentPage = 1;
		} else if (currentPage > totalPages) {
			currentPage = totalPages;
		}

		let startPage, endPage;

		if (totalPages <= maxPages) {
			// total pages less than max so show all pages
			startPage = 1;
			endPage = totalPages;
		} else {
			// total pages more than max so calculate start and end pages
			let maxPagesBeforeCurrentPage = Math.floor(maxPages / 2);
			let maxPagesAfterCurrentPage = Math.ceil(maxPages / 2) - 1;
			if (currentPage <= maxPagesBeforeCurrentPage) {
				// current page near the start
				startPage = 1;
				endPage = maxPages;
			} else if (currentPage + maxPagesAfterCurrentPage >= totalPages) {
				// current page near the end
				startPage = totalPages - maxPages + 1;
				endPage = totalPages;
			} else {
				// current page somewhere in the middle
				startPage = currentPage - maxPagesBeforeCurrentPage;
				endPage = currentPage + maxPagesAfterCurrentPage;
			}
		}

		// calculate start and end item indexes
		let startIndex = (currentPage - 1) * pageSize;
		let endIndex = Math.min(startIndex + pageSize - 1, totalItems - 1);

		// create an array of pages to ng-repeat in the pager control
		let pages = Array.from(Array((endPage + 1) - startPage).keys()).map(i => startPage + i);

		// return object with all pager properties required by the view
		return {
			totalItems: totalItems,
			currentPage: currentPage,
			pageSize: pageSize,
			totalPages: totalPages,
			startPage: startPage,
			endPage: endPage,
			startIndex: startIndex,
			endIndex: endIndex,
			pages: pages
		}
	}

	handlePageChange = (newPage) => {
		this.props.handlePageChange(newPage);
	}

	render() {
		const { currentPage, totalPages, pages } = this.paginate();

		return (
			<Pagination>
				<PaginationItem disabled={currentPage == 1} onClick={() => currentPage > 1 && this.handlePageChange(1)}>
					<PaginationLink first />
				</PaginationItem>
				<PaginationItem disabled={currentPage == 1} onClick={() => currentPage > 1 && this.handlePageChange(currentPage - 1)} >
					<PaginationLink previous/>
				</PaginationItem>

				{pages.map((value, index) =>
					<PaginationItem active={value === currentPage} key={value} onClick={() => this.handlePageChange(value)}>
						<PaginationLink>
							{value}
						</PaginationLink>
					</PaginationItem>
				)}

				<PaginationItem disabled={currentPage == totalPages} onClick={() => currentPage < totalPages && this.handlePageChange(currentPage + 1)} >
					<PaginationLink next/>
				</PaginationItem>
				<PaginationItem disabled={currentPage == totalPages} onClick={() => currentPage < totalPages && this.handlePageChange(totalPages)}>
					<PaginationLink last/>
				</PaginationItem>
				</Pagination>
			)
	}
}